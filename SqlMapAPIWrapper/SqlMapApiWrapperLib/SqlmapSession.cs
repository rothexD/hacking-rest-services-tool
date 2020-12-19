using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SqlMapAPIWrapperLib
{
    /// <summary>
    /// Class for Handling Http Requests to the api
    /// </summary>
    public class SqlmapSession : IDisposable
    {
        private string _host = string.Empty;
        private int _port = 8775; //default port
        private static readonly HttpClient _client = new HttpClient();
        public SqlmapSession(string host, int port = 8775)
        {
            _host = host;
            _port = port;
        }
        public async Task<string> ExecuteGet(string url)
        {
            return  await _client.GetStringAsync("http://" + _host + ":" + _port + url);
        }
        public async Task<string> ExecutePost(string url, string data)
        {
            StringContent content = new StringContent(data,Encoding.UTF8,"application/json");
            var response = await _client.PostAsync("http://" + _host + ":" + _port + url, content);
            return await response.Content.ReadAsStringAsync();
        }
        public void Dispose()
        {
            _host = null;
        }
    }
}