using System;
using System.Collections.Generic;
using System.Net.Http;
using Hacking_REST_Services.ServiceHandlers;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers;
using Hacking_Rest_SqlInjetor.WebClient;
using Hacking_REST_Services.Helpers;
using Hacking_Rest_SqlInjetor.Form;

namespace Hacking_Rest_SqlInjetor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string loginPage = "http://127.0.0.1:55001/login.php";
            const string xssPage = "http://127.0.0.1:55001/xss_get.php";
            ICustomHttpClient Client = new CustomHttpClient();
        }
    }
}
