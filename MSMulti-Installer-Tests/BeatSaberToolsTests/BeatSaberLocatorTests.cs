using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using BSMulti_Installer2.Utilities;
using System.Linq;

namespace MSMulti_Installer_Tests.BeatSaberToolsTests
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
    }
}
