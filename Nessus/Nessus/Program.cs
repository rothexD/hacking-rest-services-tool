using System;
using System.Threading;
using Nessus.Nessus;

namespace Nessus
{
    class Program
    {
        static void Main(string[] args)
        {
            const string targetIp = "xxx.xxx.xxx.xxx";  // to be scanned
            const string scanName = "basic";
         
            // start session
            var session = new NessusSession("localhost", "myuser", "mypassword");
            var scanner = new NessusScanner(session);
            
            // choose template by name and start scan
            var templates = scanner.GetTemplates()["templates"];
            int scanId = 0;

            foreach (var template in templates)
            {
                string name = template["name"].ToString();

                if (name != scanName)
                {
                    continue;
                }

                string tUuid = template["uuid"].ToString();
                string tDescription = template["desc"].ToString();
                var createdScan = scanner.CreateScan(tUuid, targetIp, name, tDescription)["scan"];
                
                string createdScanIdStr = createdScan["id"].ToString();
                scanId = int.Parse(createdScanIdStr);
                scanner.StartScan(scanId);
            }

            // wait for test to complete
            var results = scanner.GetScanResult(scanId);
            var info = results["info"];
            Console.WriteLine("Waiting for completion ...");

            while (info["status"].ToString() == "running")
            {
                Thread.Sleep(5000);
                results = scanner.GetScanResult(scanId);
                info = results["info"];
            }
            
            // output found vulnerabilities
            var vulnerabilities = results["vulnerabilities"];
            
            foreach (var vulnerability in vulnerabilities)
            {
                string severityString = vulnerability["severity"].ToString();
                int severity = int.Parse(severityString);

                string severityLevel = severity switch
                {
                    0 => "Info",
                    1 => "Low",
                    2 => "Medium",
                    3 => "High",
                    4 => "Critical",
                    _ => "Unknown"
                };

                if (severityLevel == "Info")
                {
                    continue;
                }

                string text = vulnerability["plugin_name"].ToString();
                string message = $"[{severityLevel}] {text}";
                Console.WriteLine(message);
            }
            
            // close session
            session.Logout();
        }
    }
}