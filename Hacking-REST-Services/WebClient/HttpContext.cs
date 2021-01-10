using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Hacking_REST_Services.WebClient
{
    public class HttpContext : AbstractHttpContext
    {
        internal Dictionary<string, string> Fields { get; set; }

        public HttpContext()
        {
            Fields = new Dictionary<string, string>();
        }

        public HttpContext(HttpMethod method, string uri) : base(method, uri)
        {
            Fields = new Dictionary<string, string>();
        }

        public HttpContext(HttpMethod method, string uri, Dictionary<string, string> fields) : base(method, uri)
        {
            Fields = fields;
        }

        public override void Reset()
        {
            Fields.Clear();
        }

        public override void AddField(string name, string value)
        {
            Fields[name] = value;
        }

        public override AbstractHttpContext BuildRequest()
        {
            if (Method == HttpMethod.Get)
            {
                string content = new FormUrlEncodedContent(Fields).ReadAsStringAsync().Result;
                string newUri = RequestUri + "?" + content;
                Console.WriteLine(newUri);
                RequestUri = new Uri(newUri);
                return this;
            }

            if (Method != HttpMethod.Post)
            {
                throw new ArgumentException("BuildRequest: Only GET and POST are supported.");
            }

            Content = new FormUrlEncodedContent(Fields);
            return this;
        }

        public string GetRequestAsHttpString(string cookie,string[] Headers)
        {
            string returnval = $"{Method} {RequestUri} HTTP/1.1\r\n";
            if (Headers != null)
            {
                foreach (string item in Headers)
                {
                    returnval += item;
                    returnval += "\r\n";
                }
            }
            
            if(cookie != null)
            {
               returnval += $"Cookie: {cookie}\r\n";
            }
            returnval += "\r\n";
            returnval += this.Content.ReadAsStringAsync().Result;

            return returnval;
        }
    }
}