using System.Net.Http;
using System.Threading.Tasks;

namespace Hacking_REST_Services.WebClient
{
    public interface ICustomHttpClient
    {
        public Task<HttpResponseMessage> Get(string uri);
        
        public Task<HttpResponseMessage> Get(AbstractHttpContext request);

        public Task<HttpResponseMessage> Post(AbstractHttpContext request);
        public Task<HttpResponseMessage> ByContextMethod(AbstractHttpContext request);

        public HttpClientHandler ClientHandler { get; }
    } 
}