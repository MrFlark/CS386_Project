using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace CS386_Project.code
{
    public class Server
    {
        public static readonly string TEMP_DIR = @"C:\temp\";

        private static List<Session> _sessions = null;
        public static List<Session> Sessions {
            get {
                if (_sessions == null)
                {
                    _sessions = new List<Session>();
                }
                return _sessions;
            }
        }

        public static Client FindClient(Guid clientId)
        {
            foreach (var session in Sessions)
            {
                foreach (var client in session.Clients)
                {
                    if (client.ClientId == clientId)
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        public static string NormalizeURL(string url)
        {
            if (url.IndexOf("?", StringComparison.InvariantCulture) != -1)
            {
                url = url.Substring(0, url.IndexOf("?", StringComparison.InvariantCulture));
            }

            return url.ToLower().Trim();
        }

        private Server()
        {
        }
    }
}
