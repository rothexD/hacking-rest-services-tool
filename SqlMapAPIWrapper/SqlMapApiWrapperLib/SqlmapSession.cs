using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SqlMapAPIWrapperLib
{
    public class SqlmapSession : IDisposable
    {
        private string _host = string.Empty;
        private int _port = 8775; //default port
        static readonly HttpClient Client = new HttpClient();
        public SqlmapSession(string host, int port = 8775)
        {
            _host = host;
            _port = port;
        }
        public async Task<string> ExecuteGet(string url)
        {
            return  await Client.GetStringAsync("http://" + _host + ":" + _port + url);
        }
        public async Task<string> ExecutePost(string url, string data)
        {
            StringContent content = new StringContent(data,Encoding.UTF8,"application/json");
            var response = await Client.PostAsync("http://" + _host + ":" + _port + url, content);
            return await response.Content.ReadAsStringAsync();
        }
        public void Dispose()
        {
            _host = null;
        }
    }
}