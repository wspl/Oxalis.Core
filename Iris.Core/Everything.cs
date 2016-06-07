using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Iris.Core
{
    class Everything
    {
        public class EverythingResult
        {
            [JilDirective(Name = "totalResults")]
            public int TotalResults { get; set; }

            [JilDirective(Name = "results")]
            public List<EverythingResultItem> Results { get; set; }
        }

        public class EverythingResultItem
        {
            [JilDirective(Name = "type")]
            public string Type { get; set; }

            [JilDirective(Name = "name")]
            public string Name { get; set; }

            [JilDirective(Name = "path")]
            public string Path { get; set; }
        }

        public Everything()
        {
            if (CheckEverythingAvailable()) return;
            if (CheckPortAvailable(15013))
            {
                // Launch Everything
                var proEverything = new Process();

                var proEverythingPath = Path.Combine(Configuration.ResourcesPath, "stein_everything.exe");
                var proEverythingArgs = "-install-service -install-client-service";

                proEverything.StartInfo = new ProcessStartInfo(proEverythingPath, proEverythingArgs);
                proEverything.Start();
            }
            else
            {
                throw new Exception("Port is unavailable.");
            }
        }

        public EverythingResult Search(string regexp)
        {
            // Use Everything's Http API
            var client = new RestClient("http://127.0.0.1:15013");
            var request = new RestRequest("/", Method.GET)
                .AddParameter("search", "regex:" + regexp)
                .AddParameter("j", 1)
                .AddParameter("p", 1)
                .AddParameter("path_column", 1);

            var response = client.Execute(request);
            var result = JSON.Deserialize<EverythingResult>(response.Content);

            return result;
        }

        private bool CheckPortAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (var tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckEverythingAvailable()
        {
            var client = new RestClient("http://127.0.0.1:15013");
            var request = new RestRequest("/Everything.gif", Method.GET);

            var response = client.Execute(request);

            if (response.ErrorException == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
