using BSMulti_Installer2.Utilities;
using BSMulti_Installer2.XML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer_Tests.UtilitiesTests
{
    [TestClass]
    public class InstallerTests
    {
        public static readonly string OutputDir = Path.Combine("Output", "Installer");
        [TestMethod]
        public async Task InstallAndruzz()
        {
#if NCRUNCH
            var config = Utilities.GetInstallerConfigFromFile(Path.Combine("Data", "MultiplayerFilesTest.xml"));
#else
            var config = Utilities.GetInstallerConfigFromFile(Path.GetFullPath(@"..\..\..\..\MultiplayerFiles.xml"));
#endif
            var mod = config.ModGroup.First(m => m.Name == "Multiplayer");
            string outputDir = Path.Combine(OutputDir, "Andruzz");
            Directory.CreateDirectory(outputDir);
            Installer installer = new Installer(outputDir, config, mod, null, true);
            installer.EnsureValidInstaller();
            Progress<OverallProgress> overallProgress = new Progress<OverallProgress>(OnOverallProgress);
            Progress<ComponentProgress> componentProgress = new Progress<ComponentProgress>(OnComponentProgress);
            await installer.InstallMod(overallProgress, componentProgress);
        }

        [TestMethod]
        public async Task InstallZinga()
        {
#if NCRUNCH
            var config = Utilities.GetInstallerConfigFromFile(Path.Combine("Data", "MultiplayerFilesTest.xml"));
#else
            var config = Utilities.GetInstallerConfigFromFile(Path.GetFullPath(@"..\..\..\..\MultiplayerFiles.xml"));
#endif
            var mod = config.ModGroup.First(m => m.Name == "MultiplayerLite");
            string outputDir = Path.Combine(OutputDir, "Zinga");
            Directory.CreateDirectory(outputDir);
            Installer installer = new Installer(outputDir, config, mod, mod.GetOptionalComponents(config), true);
            installer.EnsureValidInstaller();
            Progress<OverallProgress> overallProgress = new Progress<OverallProgress>(OnOverallProgress);
            Progress<ComponentProgress> componentProgress = new Progress<ComponentProgress>(OnComponentProgress);
            await installer.InstallMod(overallProgress, componentProgress);
        }

        [TestMethod]
        public void InstallAndruzz_IComponentInstaller()
        {
            //var config = Utilities.GetInstallerConfigFromFile(@"Data\MultiplayerFilesTest.xml");
            //var mod = config.ModGroup.First(m => m.Name == "Multiplayer");
            //string outputDir = Path.Combine(OutputDir, "Andruzz_IComponentInstaller");
            //Directory.CreateDirectory(outputDir);
            //var components = mod.GetComponents(config);
            //var sourceDir = new DirectoryInfo(Paths.Path_Temp);
            //foreach (var c in components)
            //{
            //    string name = $"{c.Name}-{c.Version}*";
            //    FileInfo file = sourceDir.GetFiles(name).First();
            //    IComponentInstaller installer = c.GetInstallation().GetInstaller();
            //    installer.Install(file.FullName, outputDir);
            //}
            
        }


            public void OnOverallProgress(OverallProgress progress)
        {
            Console.WriteLine($"Status: Downloading {progress.Component} {progress.Stage}/{progress.TotalStages}");
        }
        public void OnComponentProgress(ComponentProgress progress)
        {
            Console.WriteLine($"{progress.Component}: {progress.Progress.ToString("P")}");
        }
    }
}
