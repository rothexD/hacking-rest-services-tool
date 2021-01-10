using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace Hacking_REST_Services.WebClient
{
    public class CustomHttpClient : ICustomHttpClient
    {
        private readonly HttpClient _client;
        public HttpClientHandler ClientHandler { get; private set; }

        public CustomHttpClient()
        {
            ClientHandler = new HttpClientHandler()
            {
                //AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = new CookieContainer()              
            };
           
            _client = new HttpClient(ClientHandler);
        }

        public CustomHttpClient(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> Get(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            return _client.SendAsync(request);
        }

        public Task<HttpResponseMessage> Get(AbstractHttpContext request)
        {
            string cookie = "";
            return _client.SendAsync(request);
        }
        public Task<HttpResponseMessage> Post(AbstractHttpContext request)
        {
            string cookie = "";
            return _client.SendAsync(request);
        }

        public Task<HttpResponseMessage> ByContextMethod(AbstractHttpContext request)
        {
            if(request.Method == HttpMethod.Post)
            {
                return Post(request);
            }
            return Get(request);
        }

        public static string GetResponseContent(Task<HttpResponseMessage> response)
        {
            return GetResponseContent(response.Result);
        }

        private static string GetResponseContent(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public static HtmlDocument GetHtmlDocument(Task<HttpResponseMessage> response)
        {
            return GetHtmlDoc(response.Result);
        }

        private static HtmlDocument GetHtmlDoc(HttpResponseMessage response)
        {
            string content = GetResponseContent(response);
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }
    }
}