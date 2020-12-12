using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Hacking_Rest_SqlInjetor.WebClient
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
    }
}