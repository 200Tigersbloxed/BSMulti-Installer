
using BSMulti_Installer2.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BSMulti_Installer2.XML
{
    public partial class MultiplayerInstallerConfiguration
    {
        public static MultiplayerInstallerConfiguration Deserialize(Stream xmlStream)
        {
            var serializer = new XmlSerializer(typeof(MultiplayerInstallerConfiguration));
            MultiplayerInstallerConfiguration installerDef = (MultiplayerInstallerConfiguration)serializer.Deserialize(xmlStream);
            installerDef.OnDeserialized();
            return installerDef;
        }
        public static MultiplayerInstallerConfiguration Deserialize(string filePath)
        {
            using (var reader = File.OpenRead(filePath))
            {
                return Deserialize(reader);
            }
        }

        public MultiplayerComponent[] GetSortedDependencies(MultiplayerMod mod)
        {
            MultiplayerComponent[] unsorted = mod.GetComponents(this).ToArray();
            return unsorted.TSort(m => m.GetComponents(this), true).ToArray();
        }

        public MultiplayerComponent[] GetSortedDependencies(IEnumerable<MultiplayerComponent> components)
        {
            var unsorted = new Dictionary<string, MultiplayerComponent>();
            foreach (var c in components)
            {
                string cId = GetComponentString(c);
                if (!unsorted.ContainsKey(cId))
                {
                    unsorted.Add(cId, c);
                    foreach (var dep in c.GetComponents(this))
                    {
                        string depId = GetComponentString(dep);
                        unsorted[depId] = dep;
                    }
                }
            }


            return unsorted.Values.TSort(m => m.GetComponents(this), true).ToArray();
        }

        public MultiplayerComponent GetComponent(ComponentReference c)
        {
            if (TryGetComponent(c, out MultiplayerComponent component))
                return component;
            else
                throw new InvalidOperationException($"Component {c.Name} ({c.Version}) was not found in ComponentDefinitions.");
        }



        private Dictionary<string, MultiplayerComponent> _componentsDict;
        public static string GetComponentString(string name, string version) => $"{name}|{version}";
        public static string GetComponentString(MultiplayerComponent component) => GetComponentString(component.Name, component.Version);
        public static string GetComponentString(ComponentReference component) => GetComponentString(component.Name, component.Version);

        public bool TryGetComponent(string name, string version, out MultiplayerComponent component)
        {
            return _componentsDict.TryGetValue(GetComponentString(name, version), out component);
        }

        public bool TryGetComponent(ComponentReference componentReference, out MultiplayerComponent component)
        {
            return _componentsDict.TryGetValue(GetComponentString(componentReference), out component);
        }

        internal void OnDeserialized()
        {
            MultiplayerComponent[] components = ComponentDefinitions;
            _componentsDict = new Dictionary<string, MultiplayerComponent>(components.Length, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < components.Length; i++)
            {
                MultiplayerComponent component = components[i];
                try
                {
                    _componentsDict.Add($"{component.Name}|{component.Version}", component);
                }
                catch (ArgumentException ex)
                {
                    throw new MultiplayerInstallerSerializationException($"Error processing ComponentDefinitions: {ex.Message}", ex);
                }
            }
        }

    }

    public partial class MultiplayerMod
    {


    }


    public partial class ComponentInstallation
    {

    }


    public partial class MoveTo : IComponentInstaller
    {
        public void Install(string source, string destinationDirectory)
        {
            FileInfo file = new FileInfo(Path.Combine(destinationDirectory, Destination));

            Directory.CreateDirectory(file.DirectoryName);
            if (file.Exists)
                file.Delete();
            File.Copy(source, file.FullName);
        }
    }
    public partial class ExtractTo : IComponentInstaller
    {
        public void Install(string source, string destinationDirectory)
        {
            if (!string.IsNullOrEmpty(Directory))
                destinationDirectory = Path.Combine(destinationDirectory, Directory);
            System.IO.Directory.CreateDirectory(destinationDirectory);
            using (var fs = System.IO.File.OpenRead(source))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Read, false))
            {
                if(File == null || File.Length == 0)
                {
                    zip.ExtractToDirectory(destinationDirectory, true);
                    return;
                }
                foreach (var file in File)
                {
                    var entry = zip.GetEntry(file.ZipPath.Replace('\\', '/'));
                    FileInfo fInfo = new FileInfo(Path.Combine(destinationDirectory, file.Path));
                    System.IO.Directory.CreateDirectory(fInfo.DirectoryName);
                    entry.ExtractToFile(fInfo.FullName, true);
                }
            }
        }
    }

    public partial class MultiplayerComponent
    {
        public IComponentInstaller GetInstaller()
        {
            if (Installation?.Item is IComponentInstaller installer)
                return installer;
            else return new ExtractTo();
        }

        public override string ToString()
        {
            return MultiplayerInstallerConfiguration.GetComponentString(Name, Version);
        }
    }

    public partial class ComponentReference
    {
        public override string ToString()
        {
            return MultiplayerInstallerConfiguration.GetComponentString(Name, Version);
        }
    }

    public partial class ComponentMetadata
    {


    }

    public partial class ComponentFile
    {

    }
}