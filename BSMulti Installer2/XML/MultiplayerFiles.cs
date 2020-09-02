﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace BSMulti_Installer2.XML
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MultiplayerInstaller
    {
        public static MultiplayerInstaller Deserialize(Stream xmlStream)
        {
            var serializer = new XmlSerializer(typeof(MultiplayerInstaller));
            MultiplayerInstaller installerDef = (MultiplayerInstaller)serializer.Deserialize(xmlStream);
            installerDef.OnDeserialized();
            return installerDef;
        }
        public static MultiplayerInstaller Deserialize(string filePath)
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

        private MultiplayerMod[] modGroupField;

        private MultiplayerInstallerComponentDefinitions componentDefinitionsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("MultiplayerMod", IsNullable = false)]
        public MultiplayerMod[] ModGroup
        {
            get
            {
                return this.modGroupField;
            }
            set
            {
                this.modGroupField = value;
            }
        }

        /// <remarks/>
        public MultiplayerInstallerComponentDefinitions ComponentDefinitions
        {
            get
            {
                return this.componentDefinitionsField;
            }
            set
            {
                this.componentDefinitionsField = value;
            }
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
            MultiplayerComponent[] components = ComponentDefinitions.Component;
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "multiplayer-mod")]
    public partial class MultiplayerMod
    {

        private ComponentInstallation installationField;

        private ComponentReference[] dependenciesField;

        private ComponentReference[] optionalComponentsField;

        private ComponentMetadata metadataField;

        private string nameField;

        private string versionField;

        private string uRLField;

        private string minGameVersionField;

        private string maxGameVersionField;

        /// <remarks/>
        public ComponentInstallation Installation
        {
            get
            {
                return this.installationField;
            }
            set
            {
                this.installationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Component", IsNullable = false)]
        public ComponentReference[] Dependencies
        {
            get
            {
                return this.dependenciesField;
            }
            set
            {
                this.dependenciesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Component", IsNullable = false)]
        public ComponentReference[] OptionalComponents
        {
            get
            {
                return this.optionalComponentsField;
            }
            set
            {
                this.optionalComponentsField = value;
            }
        }

        /// <remarks/>
        public ComponentMetadata Metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string URL
        {
            get
            {
                return this.uRLField;
            }
            set
            {
                this.uRLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MinGameVersion
        {
            get
            {
                return this.minGameVersionField;
            }
            set
            {
                this.minGameVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MaxGameVersion
        {
            get
            {
                return this.maxGameVersionField;
            }
            set
            {
                this.maxGameVersionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "mp-component-installation")]
    public partial class ComponentInstallation
    {

        private string extractTo;
        private MoveTo itemField;
        private ComponentFile[] filesField;
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ExtractTo", typeof(string))]

        public string ExtractTo
        {
            get { return extractTo; }
            set { extractTo = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MoveTo", typeof(MoveTo))]
        public MoveTo MoveTo
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("File", IsNullable = false, Type = typeof(ComponentFile))]
        public ComponentFile[] Files
        {
            get
            {
                return this.filesField;
            }
            set
            {
                this.filesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "mp-component-file")]
    public partial class ComponentFile
    {
        private string path;
        private string sha1Field;
        private bool requireHashMatchField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string SHA1
        {
            get
            {
                return this.sha1Field;
            }
            set
            {
                this.sha1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public bool RequireHashMatch
        {
            get { return requireHashMatchField; }
            set { requireHashMatchField = value; }
        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MoveTo
    {

        private string filenameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Filename
        {
            get
            {
                return this.filenameField;
            }
            set
            {
                this.filenameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string DestinationDirectory
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "mp-component")]
    public partial class MultiplayerComponent
    {

        private ComponentInstallation installationField;

        private ComponentReference[] requiresField;

        private ComponentMetadata metadataField;

        private string nameField;

        private string versionField;

        private string uRLField;

        /// <remarks/>
        public ComponentInstallation Installation
        {
            get
            {
                return this.installationField;
            }
            set
            {
                this.installationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Requirement", IsNullable = false)]
        public ComponentReference[] Requires
        {
            get
            {
                return this.requiresField;
            }
            set
            {
                this.requiresField = value;
            }
        }

        /// <remarks/>
        public ComponentMetadata Metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string URL
        {
            get
            {
                return this.uRLField;
            }
            set
            {
                this.uRLField = value;
            }
        }

        public override string ToString()
        {
            return MultiplayerInstaller.GetComponentString(Name, Version);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "mp-component-reference")]
    public partial class ComponentReference
    {

        private string nameField;

        private string versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        public override string ToString()
        {
            return MultiplayerInstaller.GetComponentString(Name, Version);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "mp-component-metadata")]
    public partial class ComponentMetadata
    {

        private string descriptionField;

        private string notesField;

        private string linkField;

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        public string Link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MultiplayerInstallerComponentDefinitions
    {

        private MultiplayerComponent[] componentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Component")]
        public MultiplayerComponent[] Component
        {
            get
            {
                return this.componentField;
            }
            set
            {
                this.componentField = value;
            }
        }
    }

}