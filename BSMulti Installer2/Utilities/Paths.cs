using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2.Utilities
{
    public static class Paths
    {
        public static readonly Uri URL_ReleasePage = new Uri("https://github.com/200Tigersbloxed/BSMulti-Installer/releases/latest");
        public static readonly Uri URL_ReadmePage = new Uri("https://github.com/200Tigersbloxed/BSMulti-Installer/blob/master/README.md");
        public static readonly Uri URL_ReleaseAPIPage = new Uri("https://api.github.com/repos/200Tigersbloxed/BSMulti-Installer/releases");
        public static readonly Uri URL_TigerServer = new Uri("https://tigersserver.xyz");
        public static readonly Uri URL_DeveloperMessage = new Uri("https://pastebin.com/raw/vaXRephy");
        public static readonly Uri URL_ConnectionCheck = new Uri("http://google.com/generate_204");
        public static readonly Uri URL_MultiplayerConfiguration = new Uri("https://raw.githubusercontent.com/Zingabopp/BSMulti-Installer/refactor/MultiplayerFiles.xml");

        public const string Path_CachedConfig = "MultiplayerFiles.xml";
        public const string Path_Managed = @"Beat Saber_Data\Managed";
        public const string Path_Libs = "Libs";
        public const string Path_Plugins = "Plugins";
        public const string Path_PendingPlugins = @"IPA\Pending\Plugins";
        public const string Path_IPA = "IPA";
    }
}
