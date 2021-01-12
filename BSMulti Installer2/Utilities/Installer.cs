using BSMulti_Installer2.XML;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BSMulti_Installer2.Utilities
{
    public class Installer
    {
        public string InstallDirectory => UsePending ? Path.Combine(GameDirectory, Paths.Path_Pending) : GameDirectory;
        public string GameDirectory { get; protected set; }
        public string TempDirectory { get; protected set; }
        public MultiplayerInstallerConfiguration Configuration { get; protected set; }
        public MultiplayerMod SelectedMod { get; protected set; }
        private List<MultiplayerComponent> IncludedOptionals = new List<MultiplayerComponent>();
        public bool UsePending { get; protected set; }
        public Installer(string gameDirectory, MultiplayerInstallerConfiguration configuration, MultiplayerMod selectedMod,
            IEnumerable<MultiplayerComponent> includedOptionals = null, bool usePending = true)
        {
            GameDirectory = gameDirectory;
            Configuration = configuration;
            UsePending = usePending;
            SelectedMod = selectedMod;
            if (includedOptionals != null)
            {
                foreach (var item in includedOptionals)
                {
                    IncludedOptionals.Add(item);
                }
            }
        }

        /// <summary>
        /// Verifies the installer is valid.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ValidationException"></exception>
        public void EnsureValidInstaller()
        {
            if (!Directory.Exists(GameDirectory))
                throw new DirectoryNotFoundException($"Game directory not found: '{GameDirectory}'");

            Validator.ValidateMod(Configuration, SelectedMod);
        }

        public Task InstallMod(CancellationToken cancellationToken = default) => InstallMod(null, null, cancellationToken);

        protected List<string> TempFiles;

        public async Task InstallMod(IProgress<OverallProgress> overallProgress, IProgress<ComponentProgress> componentProgress,
            CancellationToken cancellationToken = default)
        {
            if (UsePending)
                Directory.CreateDirectory(InstallDirectory);
            string installDir = InstallDirectory;
            var config = Configuration;
            var selectedMod = SelectedMod;
            string tempDir = Paths.Path_Temp;
            try
            {
                Directory.CreateDirectory(tempDir);
            }
            catch (Exception ex)
            {
                throw new InstallationException($"Could not create temp directory '{tempDir}': {ex.Message}", ex);
            }
            TempDirectory = tempDir;
            TempFiles = new List<string>();
            if (!Directory.Exists(installDir))
                throw new InstallationException($"Install directory not found: '{installDir}'");
            HashSet<MultiplayerComponent> allComponents = new HashSet<MultiplayerComponent>();
            foreach (var item in selectedMod.GetComponents(config))
            {
                allComponents.Add(item);
            }
            foreach (var item in IncludedOptionals)
            {
                allComponents.Add(item);
                foreach (var dep in item.GetComponents(config))
                {
                    allComponents.Add(dep);
                }
            }
            var components = config.GetSortedDependencies(allComponents);

            int numDownloads = allComponents.Count + 1;
            int stage = 0;
            for (int i = 0; i < components.Length; i++)
            {
                stage++;
                MultiplayerComponent c = components[i];
                try
                {
                    overallProgress?.Report(new OverallProgress(c.Name, stage, numDownloads));
                    string filePath = await DownloadComponent(c.Name, c.Version, c.URL, tempDir, componentProgress, cancellationToken);
                    TempFiles.Add(filePath);
                    InstallComponent(c.GetInstallation(), c.Name, filePath);
                }
                catch (Exception ex)
                {
                    throw new ComponentInstallationException(c, ex.Message, ex);
                }
            }

            MultiplayerMod mod = SelectedMod;
            overallProgress?.Report(new OverallProgress(mod.Name, numDownloads, numDownloads));
            try
            {
                string filePath = await DownloadComponent(mod.Name, mod.Version, mod.URL, tempDir, componentProgress, cancellationToken);
                TempFiles.Add(filePath);
                InstallComponent(mod.GetInstallation(), mod.Name, filePath);
            }
            catch (InstallationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InstallationException($"Error installing '{mod.Name}': {ex.Message}", ex);
            }
            CleanupTemp();
        }

        private void InstallComponent(ComponentInstallation installation, string name, string sourceFile)
        {
            IComponentInstaller installer = installation.GetInstaller();
            installer.Install(sourceFile, InstallDirectory);
            var componentFiles = installation?.Files;
            if (componentFiles != null && componentFiles.Length > 0)
            {
                foreach (var compFile in componentFiles)
                {
                    string file = Path.Combine(InstallDirectory, compFile.Path);
                    if (!File.Exists(file))
                        throw new FileNotFoundException($"Component '{name}' did not install correctly, missing '{file}'.");
                    if (!string.IsNullOrEmpty(compFile.SHA1))
                    {
                        using (Stream fs = File.OpenRead(file))
                        {
                            string fileHash = Utilities.CreateSha1(fs);
                            if (!compFile.SHA1.Equals(fileHash, StringComparison.OrdinalIgnoreCase))
                                throw new InvalidDataException($"Component '{name}' did not install correctly, file hash of '{file}' did not match what was expected.");
                        }
                    }
                }
            }
        }

        protected void CleanupTemp()
        {
            string[] files = TempFiles.ToArray();
            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch { }
            }
        }

        private async Task<string> DownloadComponent(string name, string version, string url, string directory,
            IProgress<ComponentProgress> componentProgress, CancellationToken cancellationToken)
        {
            Uri uri = new Uri(url);
            string compName = $"{name}-{version}";
            string filePath = Path.Combine(directory, compName);
            void ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
            {
                componentProgress?.Report(new ComponentProgress(compName, progressPercentage ?? -1));
            }
            using (HttpClientDownloadWithProgress download = new HttpClientDownloadWithProgress(WebUtils.HttpClient, uri, filePath))
            {
                download.AddExtensionToPath = true;
                download.ProgressChanged += ProgressChanged;
                filePath = await download.StartDownload(cancellationToken);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File download failed for '{compName}'.");

                download.ProgressChanged -= ProgressChanged;
            }
            return filePath;
        }

    }

    public struct ComponentProgress
    {
        public string Component;
        public double Progress;
        public ComponentProgress(string component, double progress)
        {
            Component = component;
            Progress = progress;
        }
    }

    public struct OverallProgress
    {
        public string Component;
        public int Stage;
        public int TotalStages;
        public OverallProgress(string component, int stage, int totalStages)
        {
            Component = component;
            Stage = stage;
            TotalStages = totalStages;
        }
    }

#pragma warning disable CA2237 // Mark ISerializable types with serializable
    public class InstallationException : Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
    {
        public InstallationException()
        {
        }

        public InstallationException(string message) : base(message)
        {
        }

        public InstallationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class ComponentInstallationException : InstallationException
    {
        public string ComponentName { get; protected set; }
        public string ComponentVersion { get; protected set; }

        public ComponentInstallationException()
        {
        }

        public ComponentInstallationException(string message) : base(message)
        {
        }

        public ComponentInstallationException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public ComponentInstallationException(MultiplayerComponent component, string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            ComponentName = component.Name;
            ComponentVersion = component.Version;
        }
    }
}
