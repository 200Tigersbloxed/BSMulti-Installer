using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace BSMulti_Installer2.Utilities
{
    public struct GithubVersion
    {
        public bool IsSet;
        public int Major;
        public int Minor;
        public int Build;
        public int? Revision;
        public string Meta;
        public DateTime ReleaseDate;
        public void SetVersions(int[] versionAry)
        {
            if (versionAry.Length >= 3)
            {
                Major = versionAry[0];
                Minor = versionAry[1];
                Build = versionAry[2];
            }
            if (versionAry.Length >= 4)
                Revision = versionAry[3];
            IsSet = true;
        }

        public int[] GetVersionArray()
        {
            int[] ary;
            if (Revision != null)
            {
                ary = new int[4];
                ary[3] = Revision.Value;
            }
            else
                ary = new int[3];
            ary[0] = Major;
            ary[1] = Minor;
            ary[2] = Build;
            return ary;
        }

        public override string ToString()
        {
            if (Revision != null)
                return $"{Major}.{Minor}.{Build}.{Revision}";
            else
                return $"{Major}.{Minor}.{Build}";
        }
    }
    public static class VersionCheck
    {
        public static readonly string Tag_Key = "tag_name";
        public static readonly string Created_Key = "created_at";
        public static Regex VersionRegex = new Regex(@"^.*?(0|[0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:\.(0|[1-9][0-9]*))?(?:[-_\.]?((?:0|[1-9][0-9]*|[0-9]*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9][0-9]*|[0-9]*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.Compiled);

        /// <summary>
        /// Retrieves the latest release version from a GitHub API page.
        /// URI should be in the form https://api.github.com/repos/GITHUB_USER/REPOSITORY_NAME/releases
        /// </summary>
        /// <param name="releasePageUri"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public static async Task<GithubVersion> GetLatestVersionAsync(Uri releasePageUri)
        {
            HttpWebRequest request = WebRequest.CreateHttp(releasePageUri);
            request.UserAgent = "VersionChecker/1.0.0";
            WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                string pageText = await sr.ReadToEndAsync().ConfigureAwait(false);
                JSONNode releases = JSON.Parse(pageText);
                JSONNode latestRelease = releases[0];
                string tagLine = latestRelease[Tag_Key].Value;
                Console.WriteLine(tagLine);
                GithubVersion version = ParseVersion(tagLine);
                version.ReleaseDate = DateTime.Parse(latestRelease[Created_Key].Value);
                return version;
            }
        }

        /// <summary>
        /// Parses a <see cref="GithubVersion"/> from the line containing the tag number.
        /// </summary>
        /// <param name="tagLine"></param>
        /// <returns></returns>
        public static GithubVersion ParseVersion(string tagLine)
        {
            var match = VersionRegex.Match(tagLine);
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
                GithubVersion version = new GithubVersion();
                version.SetVersions(versions.ToArray());
                if (match.Groups[5].Success)
                {
                    version.Meta = match.Groups[5].Value;
                }

                return version;
            }
            else
                throw new InvalidDataException($"Could not parse a version from string '{tagLine}'");
        }

    }
}
