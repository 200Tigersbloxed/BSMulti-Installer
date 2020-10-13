using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BSMulti_Installer2.Utilities;
using System.Linq;

namespace BSMulti_Installer_Tests.UtilitiesTests
{
    [TestClass]
    public class BeatSaberLocatorTests
    {
        [TestMethod]
        public void GetBeatSaberInstalls()
        {
            var installs = BeatSaberTools.GetBeatSaberPathsFromRegistry();
            Console.WriteLine(string.Join("\n", installs.Select(i => $"{i.InstallType,-6} | {i.InstallPath}")));
        }
        [TestMethod]
        public void GetGameVersion()
        {
            var installs = BeatSaberTools.GetBeatSaberPathsFromRegistry();
            if(installs.Length > 0)
            {
                string gameDir = installs[0].InstallPath;
                string version = BeatSaberTools.GetVersion(gameDir);
                Console.WriteLine($"Game version from '{gameDir}':");
                Console.WriteLine(version);
                Assert.IsFalse(version == null || version.Length == 0);
            }
        }
    }
}
