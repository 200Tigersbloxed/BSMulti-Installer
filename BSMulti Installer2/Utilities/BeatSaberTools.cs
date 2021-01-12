﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSMulti_Installer2.Utilities.Paths;

namespace BSMulti_Installer2.Utilities
{
    public static class BeatSaberTools
    {
        // Using Path.Combine makes it safe for regions that don't use '\' as a directory separator?
        private static readonly string STEAM_REG_KEY = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall", "Steam App 620980");
        //private const string STEAM_REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 620980";
        private static readonly string OCULUS_LM_KEY = Path.Combine("SOFTWARE", "WOW6432Node", "Oculus VR, LLC", "Oculus", "Config");
        private static readonly string OCULUS_CU_KEY = Path.Combine("SOFTWARE", "Oculus VR, LLC", "Oculus", "Libraries");
        //private const string OCULUS_REG_KEY = @"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus\Config";
        public static BeatSaberInstall[] GetBeatSaberPathsFromRegistry()
        {
            var installList = new List<BeatSaberInstall>();
            var steamInstalls = GetSteamBeatSaberInstalls();
            for (int i = 0; i < steamInstalls.Length; i++)
            {
                if (!installList.Any(instl => instl.InstallPath.Equals(steamInstalls[i].InstallPath)))
                    installList.Add(steamInstalls[i]);
            }
            var oculusInstalls = GetOculusBeatSaberInstalls();
            for (int i = 0; i < oculusInstalls.Length; i++)
            {
                if (!installList.Any(instl => instl.InstallPath.Equals(oculusInstalls[i].InstallPath)))
                    installList.Add(oculusInstalls[i]);
            }
            return installList.ToArray();
        }

        public static string FindBeatSaberInOculusLibrary(string oculusLibraryPath)
        {
            string possibleLocation = Path.Combine(oculusLibraryPath, "hyperbolic-magnetism-beat-saber");
            string matchedLocation = null;
            if (Directory.Exists(possibleLocation))
            {
                if (IsBeatSaberDirectory(possibleLocation))
                    return possibleLocation;
            }
            else
            {
                string softwareFolder = Path.Combine(oculusLibraryPath, "Software");
                if (Directory.Exists(softwareFolder))
                    matchedLocation = FindBeatSaberInOculusLibrary(softwareFolder);
            }
            return matchedLocation;
        }

        public static BeatSaberInstall[] GetSteamBeatSaberInstalls()
        {
            List<BeatSaberInstall> installList = new List<BeatSaberInstall>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))// Doesn't work in 32 bit mode without this
            {
                using (var steamKey = hklm?.OpenSubKey(STEAM_REG_KEY))
                {
                    var path = (string)steamKey?.GetValue("InstallLocation", string.Empty);
                    if (IsBeatSaberDirectory(path))
                        installList.Add(new BeatSaberInstall(path, InstallType.Steam));
                }
            }
            return installList.ToArray();
        }

        public static BeatSaberInstall[] GetOculusBeatSaberInstalls()
        {
            List<BeatSaberInstall> installList = new List<BeatSaberInstall>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) // Doesn't work in 32 bit mode without this
            {
                using (var oculusKey = hklm?.OpenSubKey(OCULUS_LM_KEY))
                {
                    var path = (string)oculusKey?.GetValue("InitialAppLibrary", string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        string matchedLocation = FindBeatSaberInOculusLibrary(path);
                        if (!string.IsNullOrEmpty(matchedLocation) && !installList.Any(i => i.InstallPath == matchedLocation))
                            installList.Add(new BeatSaberInstall(matchedLocation, InstallType.Oculus));
                    }
                }
            }
            using (RegistryKey hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)) // Doesn't work in 32 bit mode without this
            {
                using (RegistryKey oculusKey = hkcu?.OpenSubKey(OCULUS_CU_KEY))
                {
                    if (oculusKey != null && oculusKey.SubKeyCount > 0)
                    {
                        foreach (var libraryKeyName in oculusKey.GetSubKeyNames())
                        {
                            using (RegistryKey library = oculusKey.OpenSubKey(libraryKeyName))
                            {
                                var path = (string)library?.GetValue("OriginalPath", string.Empty);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    string matchedLocation = FindBeatSaberInOculusLibrary(path);
                                    if (!string.IsNullOrEmpty(matchedLocation) && !installList.Any(i => i.InstallPath == matchedLocation))
                                        installList.Add(new BeatSaberInstall(matchedLocation, InstallType.Oculus));
                                }
                            }
                        }
                    }
                }
            }
            return installList.ToArray();
        }

        public static readonly char[] IllegalCharacters = new char[]
            {
                '<', '>', ':', '/', '\\', '|', '?', '*', '"',
                '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007',
                '\u0008', '\u0009', '\u000a', '\u000b', '\u000c', '\u000d', '\u000e', '\u000d',
                '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016',
                '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d', '\u001f',
            };
        /// <summary>
        /// Attempts to get the Beat Saber game version from the given install directory. Returns null if it fails.
        /// </summary>
        /// <remarks>
        /// Uses a modified implementation of https://github.com/Assistant/ModAssistant
        /// </remarks>
        /// <param name="gameDir"></param>
        /// <returns></returns>
        public static string GetVersion(string gameDir, out Exception exception)
        {
            exception = null;
            string filename = Path.Combine(gameDir, "Beat Saber_Data", "globalgamemanagers");
            if (!File.Exists(filename))
                return null;
            try
            {
                byte[] file = File.ReadAllBytes(filename);
                string str = Encoding.Default.GetString(file);
                string versionLocation = "public.app-category.games";
                int startIndex = str.IndexOfAny(Numbers, str.IndexOf(versionLocation));
                int length = str.IndexOfAny(IllegalCharacters, startIndex) - startIndex;
                string version = str.Substring(startIndex, length);

                return version;
            }
            catch(Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        public static char[] Numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static string GetVersion(string gameDir) => GetVersion(gameDir, out _);

        public static bool IsBeatSaberDirectory(string path)
        {
            if (string.IsNullOrEmpty(path?.Trim()))
                return false;
            try
            {
                DirectoryInfo bsDir = new DirectoryInfo(path); if (bsDir.Exists)
                {
                    var files = bsDir.GetFiles("Beat Saber.exe");
                    return files.Count() > 0;
                }
            }
            catch { }

            return false;
        }
    }
}
