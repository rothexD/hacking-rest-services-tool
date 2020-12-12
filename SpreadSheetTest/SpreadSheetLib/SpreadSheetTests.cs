using System;
using System.Net.Http;

namespace SpreadSheetLib
{
    public class SpreadSheetTests
    {
        private HttpClientHandler _handler;
        private HttpClient _client;
        public string RootUrl { get; set; }

        SpreadSheetTests(string rootUrl)
        {
            RootUrl = rootUrl;
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);
        }
        
        public bool RegisterUser()
        {
            
        }
        
    }
}