using BSMulti_Installer2.XML;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BSMulti_Installer2.Utilities
{
    public static class Utilities
    {
        public static Regex VersionRegex = new Regex(@"^.*?(0|[0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:\.(0|[1-9][0-9]*))?(?:[-_\.]?((?:0|[1-9][0-9]*|[0-9]*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9][0-9]*|[0-9]*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.Compiled);

        /// <summary>
        /// Compares left to right. Returns -1 if right is less than left. Returns 1 if right is greater than left.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int CompareVersions(int[] left, int[] right)
        {
            if (left == null) throw new ArgumentNullException($"Null given for '{nameof(left)}'.");
            if (right == null) throw new ArgumentNullException($"Null given for '{nameof(right)}'.");
            if (left.Length == 0) throw new ArgumentException($"Empty array given for '{nameof(left)}'.");
            if (right.Length == 0) throw new ArgumentException($"Empty array given for '{nameof(right)}'.");
            int i = 0;
            for (i = 0; i < left.Length; i++)
            {
                if (i < right.Length)
                {
                    if (left[i] > right[i])
                        return -1;
                    else if (left[i] < right[i])
                        return 1;
                }
                else if (left[i] != 0)
                    return -1;
            }
            if (i < right.Length && right[i] > 0)
                return 1;
            return 0;
        }
        /// <summary>
        /// Compares left to right. Returns -1 if right is less than left. Returns 1 if right is greater than left.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int CompareVersions(string left, int[] right) => CompareVersions(GetVersionArray(left), right);
        /// <summary>
        /// Compares left to right. Returns -1 if right is less than left. Returns 1 if right is greater than left.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int CompareVersions(Version left, int[] right) => CompareVersions(new int[] { left.Major, left.Minor, left.Build, left.Revision }, right);

        public static int[] GetVersionArray(string versionText)
        {
            var match = VersionRegex.Match(versionText);
            if (match.Success)
            {
                List<int> versions = new List<int>();
                for (int i = 1; i < 5; i++)
                {
                    if (match.Groups[i].Success)
                    {
                        versions.Add(int.Parse(match.Groups[i].Value));
                    }
                }
                int[] version = versions.ToArray();

                return version;
            }
            else
                return null;
        }

        public static string CreateSha1(Stream input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(input);

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }

        public static async Task<MultiplayerInstallerConfiguration> GetInstallerConfigFromWeb(Uri uri)
        {
            var response = await WebUtils.HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) return null;
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return MultiplayerInstallerConfiguration.Deserialize(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            }
        }
        public static MultiplayerInstallerConfiguration GetInstallerConfigFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException($"File at '{path}' could not be found.", nameof(path));
            using (var responseStream = File.OpenRead(path))
            {
                return MultiplayerInstallerConfiguration.Deserialize(responseStream);
            }
        }

        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);
                Directory.CreateDirectory(directory);

                if (file.Name != "")
                    file.ExtractToFile(completeFileName, true);
            }
        }

        public static ZipExtractResult ExtractZip(string zipPath, string extractDirectory, bool overwriteTarget = true)
        {
            if (string.IsNullOrEmpty(zipPath))
                throw new ArgumentNullException(nameof(zipPath));
            FileInfo zipFile = new FileInfo(zipPath);
            if (!zipFile.Exists)
                throw new ArgumentException($"File at zipPath {zipFile.FullName} does not exist.", nameof(zipPath));
            using (FileStream fs = zipFile.OpenRead())
            {
                return ExtractZip(fs, extractDirectory, overwriteTarget);
            }
        }

        /// <summary>
        /// Extracts a zip file to the specified directory. If an exception is thrown during extraction, it is stored in ZipExtractResult.
        /// </summary>
        /// <param name="zipPath">Path to zip file</param>
        /// <param name="extractDirectory">Directory to extract to</param>
        /// <param name="deleteZip">If true, deletes zip file after extraction</param>
        /// <param name="overwriteTarget">If true, overwrites existing files with the zip's contents</param>
        /// <returns></returns>
        public static ZipExtractResult ExtractZip(Stream zipStream, string extractDirectory, bool overwriteTarget = true, string sourcePath = null)
        {
            if (zipStream == null)
                throw new ArgumentNullException(nameof(zipStream));
            if (string.IsNullOrEmpty(extractDirectory))
                throw new ArgumentNullException(nameof(extractDirectory));

            ZipExtractResult result = new ZipExtractResult
            {
                SourceZip = sourcePath ?? "Stream",
                ResultStatus = ZipExtractResultStatus.Unknown
            };

            string createdDirectory = null;
            List<string> createdFiles = new List<string>();
            try
            {
                //Logger.log?.Info($"ExtractDirectory is {extractDirectory}");
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    //Logger.log?.Info("Zip opened");
                    //extractDirectory = GetValidPath(extractDirectory, zipArchive.Entries.Select(e => e.Name).ToArray(), shortDirName, overwriteTarget);
                    int longestEntryName = zipArchive.Entries.Select(e => e.Name).Max(n => n.Length);
                    try
                    {
                        extractDirectory = Path.GetFullPath(extractDirectory); // Could theoretically throw an exception: Argument/ArgumentNull/Security/NotSupported/PathTooLong
                    }
                    catch (PathTooLongException ex)
                    {
                        result.Exception = ex;
                        result.ResultStatus = ZipExtractResultStatus.DestinationFailed;
                        return result;
                    }
                    result.OutputDirectory = extractDirectory;
                    bool extractDirectoryExists = Directory.Exists(extractDirectory);
                    string toBeCreated = extractDirectoryExists ? null : extractDirectory; // For cleanup
                    try { Directory.CreateDirectory(extractDirectory); }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                        result.ResultStatus = ZipExtractResultStatus.DestinationFailed;
                        return result;
                    }

                    result.CreatedOutputDirectory = !extractDirectoryExists;
                    createdDirectory = string.IsNullOrEmpty(toBeCreated) ? null : extractDirectory;
                    // TODO: Ordering so largest files extracted first. If the extraction is interrupted, theoretically the song's hash won't match Beat Saver's.
                    foreach (ZipArchiveEntry entry in zipArchive.Entries.OrderByDescending(e => e.Length))
                    {
                        //if (!entry.FullName.Equals(entry.Name)) // If false, the entry is a directory or file nested in one
                        //    continue;
                        if (entry.Length == 0)
                        {
                            continue; // Directory
                        }
                        string entryPath = Path.Combine(extractDirectory, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                        bool fileExists = File.Exists(entryPath);
                        if (overwriteTarget || !fileExists)
                        {
                            try
                            {
                                entry.ExtractToFile(entryPath, overwriteTarget);
                                createdFiles.Add(entryPath);
                            }
                            catch (InvalidDataException ex) // Entry is missing, corrupt, or compression method isn't supported
                            {
                                result.Exception = ex;
                                result.ResultStatus = ZipExtractResultStatus.SourceFailed;
                                result.ExtractedFiles = createdFiles.ToArray();
                            }
                            catch (Exception ex)
                            {
                                result.Exception = ex;
                                result.ResultStatus = ZipExtractResultStatus.DestinationFailed;
                                result.ExtractedFiles = createdFiles.ToArray();

                            }
                            if (result.Exception != null)
                            {
                                foreach (string file in createdFiles)
                                {
                                    TryDelete(file);
                                }
                                return result;
                            }
                        }
                    }
                    result.ExtractedFiles = createdFiles.ToArray();
                }
                result.ResultStatus = ZipExtractResultStatus.Success;
                return result;
#pragma warning disable CA1031 // Do not catch general exception types
            }
            catch (InvalidDataException ex) // FileStream is not in the zip archive format.
            {
                result.ResultStatus = ZipExtractResultStatus.SourceFailed;
                result.Exception = ex;
                return result;
            }
            catch (Exception ex) // If exception is thrown here, it probably happened when the FileStream was opened.
#pragma warning restore CA1031 // Do not catch general exception types
            {
                try
                {
                    if (!string.IsNullOrEmpty(createdDirectory))
                    {
                        Directory.Delete(createdDirectory, true);
                    }
                    else // TODO: What is this doing here...
                    {
                        foreach (string file in createdFiles)
                        {
                            File.Delete(file);
                        }
                    }
                }
                catch (Exception cleanUpException)
                {
                    // Failed at cleanup
                }

                result.Exception = ex;
                result.ResultStatus = ZipExtractResultStatus.SourceFailed;
                return result;
            }
        }

        public static bool TryDelete(string filePath)
        {

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class ZipExtractResult
    {
        public string SourceZip { get; set; }
        public string OutputDirectory { get; set; }
        public bool CreatedOutputDirectory { get; set; }
        public string[] ExtractedFiles { get; set; }
        public ZipExtractResultStatus ResultStatus { get; set; }
        public Exception Exception { get; set; }
    }

    public enum ZipExtractResultStatus
    {
        /// <summary>
        /// Extraction hasn't been attempted.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Extraction was successful.
        /// </summary>
        Success = 1,
        /// <summary>
        /// Problem with the zip source.
        /// </summary>
        SourceFailed = 2,
        /// <summary>
        /// Problem with the destination target.
        /// </summary>
        DestinationFailed = 3
    }
}
