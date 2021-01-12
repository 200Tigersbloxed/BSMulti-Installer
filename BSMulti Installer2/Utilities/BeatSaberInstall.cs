﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BSMulti_Installer2.Utilities
{
    public class BeatSaberInstall
    {
        public string InstallPath { get; private set; }
        public InstallType InstallType { get; private set; }

        public BeatSaberInstall(string path, InstallType installType)
        {
            InstallPath = path.TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            InstallType = installType;
        }
        public override string ToString()
        {
            return $"{InstallType.ToString()}: {InstallPath}";
        }
    }

    public enum InstallType
    {
        Manual,
        Steam,
        Oculus
    }
}
