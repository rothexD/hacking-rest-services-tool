using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Hacking_Rest_SqlInjetor.WebClient
{
    public class CustomHttpClient : ICustomHttpClient
    {
        private readonly HttpClient _client;

        public CustomHttpClient()
        {
            _client = new HttpClient();
        }

        public CustomHttpClient(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> Get(string uri)
        {
            return _client.GetAsync(uri);
        }

        public Task<HttpResponseMessage> Get(AbstractHttpContext request)
        {
            string uri = request.RequestUri.ToString();
            return _client.GetAsync(uri);
        }

        public Task<HttpResponseMessage> Post(AbstractHttpContext request)
        {
            return _client.SendAsync(request);
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