using System;
using Newtonsoft.Json.Linq;

namespace Nessus.Nessus
{
    public class NessusScanner
    {
        private readonly NessusSession _session;
        
        public NessusScanner(NessusSession session)
        {
            if (!session.Authenticated)
            {
                throw new ArgumentException("Please first authenticate before creating a scanner.");
            }
            
            _session = session;
        }

        public JObject GetTemplates()
        {
            return _session.SendRequest("GET", "/editor/policy/templates");
        }
        
        public JObject GetScans()
        {
            return _session.SendRequest("GET", "/scans");
        }

        public JObject CreateScan(string uuid, string targetIp, string name, string description = "")
        {
            var scanData = new JObject
            {
                ["uuid"] = uuid,
                ["settings"] = new JObject
                {
                    ["name"] = name,
                    ["text_targets"] = targetIp,
                    ["description"] = description
                }
            };

            return _session.SendRequest("POST", "/scans", scanData);
        }

        public JObject StartScan(int scanId)
        {
            string path = $"/scans/{scanId}/launch";
            return _session.SendRequest("POST", path);
        }

        public JObject GetScanResult(int scanId)
        {
            string path = $"/scans/{scanId}";
            return _session.SendRequest("GET", path);
        }
    }
}