using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2
{
    public class MultiplayerComponent
    {
        public readonly Uri Uri;
        public readonly string[] ExtractedFilePaths;
        public string SourceZip { get; protected set; }

        /// <summary>
        /// Installs the component at the given <paramref name="beatSaberPath"/>.
        /// </summary>
        /// <param name="beatSaberPath"></param>
        /// <returns></returns>
        public virtual async Task<bool> Install(string beatSaberPath)
        {

        }


        /// <summary>
        /// Returns true if the component's files were installed correctly at the given <paramref name="beatSaberPath"/>.
        /// Files from the component that were not installed correctly are returned in <paramref name="missingFiles"/>.
        /// </summary>
        /// <param name="beatSaberPath"></param>
        /// <param name="missingFiles"></param>
        /// <returns></returns>
        public virtual bool VerifyInstall(string beatSaberPath, out string[] missingFiles)
        {
            List<string> badFiles = new List<string>();
            foreach (var file in ExtractedFilePaths)
            {
                if (!File.Exists(Path.Combine(beatSaberPath, file)))
                    badFiles.Add(file);
            }
            if (badFiles.Count > 0)
                missingFiles = badFiles.ToArray();
            else
                missingFiles = Array.Empty<string>();
            return missingFiles.Length == 0;
        }

        public MultiplayerComponent(Uri uri, IEnumerable<string> extractedFilePaths)
        {
            Uri = uri;
            ExtractedFilePaths = extractedFilePaths.ToArray();
        }
    }
}
