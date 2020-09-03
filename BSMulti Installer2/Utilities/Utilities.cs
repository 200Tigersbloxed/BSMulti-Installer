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

        /// <summary>
        /// Parses a version string into an array of <see cref="int"/>. Returns null if the string is invalid.
        /// </summary>
        /// <param name="versionText"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a SHA1 hash from an input stream.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateSha1(Stream input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(input);

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Retrieves and deserializes a <see cref="MultiplayerInstallerConfiguration"/> XML file from a web URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<MultiplayerInstallerConfiguration> GetInstallerConfigFromWeb(Uri uri)
        {
            var response = await WebUtils.HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) return null;
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return MultiplayerInstallerConfiguration.Deserialize(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Retrieves and deserializes a <see cref="MultiplayerInstallerConfiguration"/> XML file from a file path.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static MultiplayerInstallerConfiguration GetInstallerConfigFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException($"File at '{path}' could not be found.", nameof(path));
            using (var responseStream = File.OpenRead(path))
            {
                return MultiplayerInstallerConfiguration.Deserialize(responseStream);
            }
        }

        /// <summary>
        /// Extracts the contents of a <see cref="ZipArchive"/> to a directory with an option to overwrite existing files.
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="destinationDirectoryName"></param>
        /// <param name="overwrite"></param>
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
    }
}
