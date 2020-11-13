using System;
using System.Net;
using System.Net.Http;

namespace Hacking_Rest_SqlInjetor.WebClient
{
    public abstract class AbstractHttpContext : HttpRequestMessage
    {
        public AbstractHttpContext() {}

        public AbstractHttpContext(HttpMethod method, Uri uri) : base (method, uri) {}
        
        public AbstractHttpContext(HttpMethod method, string uri) : base (method, uri) {}
        
        public abstract void Reset();

        public abstract void AddField(string name, string value);

        public abstract void BuildRequest();
    }
}