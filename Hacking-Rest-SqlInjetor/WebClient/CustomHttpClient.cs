using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hacking_Rest_SqlInjetor.WebClient
{
    public class CustomHttpClient : ICustomHttpClient
    {
        private HttpClient _client;

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
    }

    public static class CustomHttpClientExample
    {
        public static void Play()
        {
            const string loginPage = "http://localhost:40201/login.php";
            
            var request = new HttpContext
            {
                RequestUri = new Uri(loginPage),
                Method = HttpMethod.Post
            };
            
            request.AddField("login", "bee");
            request.AddField("password", "bug");
            request.AddField("security_level", "0");
            request.AddField("form", "submit");
            request.BuildRequest();
            
            var client = new CustomHttpClient();
            var response = client.Post(request).Result;
            string responseString = response.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine(responseString);
        }
    }
}