using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace Hacking_Rest_SqlInjetor.WebClient
{
    public class HttpContext : AbstractHttpContext
    {
        private  List<KeyValuePair<string, string>> _fields;

        public HttpContext()
        {
            _fields = new List<KeyValuePair<string, string>>();
        }
        
        public HttpContext(HttpMethod method, Uri uri) : base(method, uri)
        {
            _fields = new List<KeyValuePair<string, string>>();
        }
        
        public HttpContext(HttpMethod method, string uri) : base(method, uri)
        {
            _fields = new List<KeyValuePair<string, string>>();
        }

        public override void Reset()
        {
            _fields.Clear();
        }

        public override void AddField(string name, string value)
        {
            RemoveFieldsWithName(name);
            var pair = new KeyValuePair<string, string>(name, value);
            _fields.Add(pair);
        }

        public override void BuildRequest()
        {
            if (Method == HttpMethod.Get)
            {
                string content = new FormUrlEncodedContent(_fields).ReadAsStringAsync().Result;
                string newUri = RequestUri + "?" + content;
                RequestUri = new Uri(newUri);
                return;
            }

            if (Method == HttpMethod.Post)
            {
                Content = new FormUrlEncodedContent(_fields);
                return;
            }
            
            throw new ArgumentException("BuildRequest: Only GET and POST are supported.");
        }

        private void RemoveFieldsWithName(string name)
        {
            var myFields = new List<KeyValuePair<string, string>>(_fields);
            foreach (var field in myFields)
            {
                if (field.Key == name)
                {
                    _fields.Remove(field);
                }
            }
        }
    }
}