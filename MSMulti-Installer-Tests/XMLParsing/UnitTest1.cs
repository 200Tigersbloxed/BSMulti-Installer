using Microsoft.VisualStudio.TestTools.UnitTesting;
using BSMulti_Installer2.XML;
using System.IO;
using System;
using System.Linq;

namespace MSMulti_Installer_Tests.XMLParsing
{
    [TestClass]
    public class UnitTest1
    {
        public static readonly string DataPath = "Data";
        [TestMethod]
        public void TestMethod1()
        {
            MultiplayerInstaller installerDef = MultiplayerInstaller.Deserialize(Path.Combine(DataPath, "Test1.xml"));
            Validator.ValidateXML(installerDef);
            Console.WriteLine("GetSortedDependencies with MultiplayerMod:");
            var comps = installerDef.GetSortedDependencies(installerDef.ModGroup[0]);
            Console.WriteLine(string.Join("\n", comps.Select(c => c.ToString())));

            Console.WriteLine("\nGetSortedDependencies with IEnumerable<MultiplayerComponent>:");
            comps = installerDef.GetSortedDependencies(installerDef.ModGroup[0].Dependencies.Select(d => installerDef.GetComponent(d)));
            Console.WriteLine(string.Join("\n", comps.Select(c => c.ToString())));

            Console.WriteLine("\nOptional Dependencies for MultiplayerLite:");
            comps = installerDef.GetSortedDependencies(installerDef.ModGroup[1].OptionalComponents.Select(d => installerDef.GetComponent(d)));
            Console.WriteLine(string.Join("\n", comps.Select(c => c.ToString())));
        }
    }
}
