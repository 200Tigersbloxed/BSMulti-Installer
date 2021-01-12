using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2.Utilities
{
    public static class WebUtils
    {
        public static readonly string UserAgent = $"BSMulti-Installer2/{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        private static readonly object _clientLock = new object();
        private static HttpClient _client;
        public static HttpClient HttpClient
        {
            get
            {
                lock (_clientLock)
                {
                    if (_client == null)
                    {
                        _client = new HttpClient();
                        _client.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent);
                    }
                }
                return _client;
            }
        }
    }
}
