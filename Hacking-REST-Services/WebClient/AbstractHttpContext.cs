using System;
using System.Net.Http;

namespace Hacking_REST_Services.WebClient
{
    public abstract class AbstractHttpContext : HttpRequestMessage
    {
        protected AbstractHttpContext() {}

        protected AbstractHttpContext(HttpMethod method, Uri uri) : base (method, uri) {}

        protected AbstractHttpContext(HttpMethod method, string uri) : base (method, uri) {}
        
        public abstract void Reset();

        public abstract void AddField(string name, string value);

        public abstract AbstractHttpContext BuildRequest();
    }
}