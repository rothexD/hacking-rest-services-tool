using System.Net.Http;
using System.Threading.Tasks;

namespace Hacking_Rest_SqlInjetor.WebClient
{
    public interface ICustomHttpClient
    {
        public Task<HttpResponseMessage> Get(string uri);
        
        public Task<HttpResponseMessage> Get(AbstractHttpContext request);

        public Task<HttpResponseMessage> Post(AbstractHttpContext request);
    }
}