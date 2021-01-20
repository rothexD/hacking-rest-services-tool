using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Nessus.Nessus
{
    public class NessusSession
    {
        public string Host { get; }
        
        public int Port { get; }
        
        public string Token { get; private set; }
        
        public bool Authenticated => Token != null;
        
        public string ApiToken { get; private set; }
        
        public NessusSession(string host, string username, string password, int port = 8834)
        {
            ServicePointManager.ServerCertificateValidationCallback = 
                (sender, certificate, chain, errors) => true;  // accept all certificates (for self-signed ones)
            
            Host = host;
            Port = port;
            Token = null;
            
            Login(username, password);
        }

        public void Login(string username, string password)
        {
            var data = new JObject
            {
                ["username"] = username, 
                ["password"] = password
            };
            
            var result = SendRequest("POST", "/session", data);

            if (result["token"] is null)
            {
                return;
            }

            Token = result["token"].ToString();
            ApiToken = GetApiToken();
        }

        public void Logout()
        {
            if (!Authenticated)
            {
                return;
            }
            
            SendRequest("DELETE", "/session");
            Token = null;
            ApiToken = null;
        }

        public JObject SendRequest(string method, string path, JObject data = null)
        {
            if (!Authenticated && path != "/session")
            {
                throw new ArgumentException("Please first authenticate to make a request.");
            }
            
            string url = $"https://{Host}:{Port}{path}";
            var request = WebRequest.Create(url);
            
            request.Method = method;
            request.Headers["X-Cookie"] = "token=" + Token;
            request.Headers["X-API-Token"] = ApiToken;
            request.ContentType = "application/json";

            if (data != null)
            {
                WriteRequestToStream(request, data);
            }

            string response = GetResponseForRequest(request);
            return string.IsNullOrEmpty(response) ? new JObject() : JObject.Parse(response);
        }
        
        private string GetApiToken()
        {
            var request = WebRequest.Create($"https://{Host}:{Port}/nessus6.js");
            request.Method = "GET";
            string response = GetResponseForRequest(request);
            
            return Regex.Match(response,
                "([a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12})").Groups[1].Value;
        }

        private static void WriteRequestToStream(WebRequest request, object data)
        {
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(data.ToString() ?? string.Empty);
            request.ContentLength = payload.Length;
            using var writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
        }
        
        private static string GetResponseForRequest(WebRequest request)
        {
            var webResponse = request.GetResponse();
            var readStream = webResponse.GetResponseStream();
            using var reader = new StreamReader(readStream);
            return reader.ReadToEnd();
        }
    }
}