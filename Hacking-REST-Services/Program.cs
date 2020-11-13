using System;
using System.Net.Http;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string loginPage = "http://localhost:40201/login.php";
            const string xssPage = "http://localhost:40201/xss_get.php";

            // login to bWAPP
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

            ICustomHttpClient client = new CustomHttpClient();
            client.Post(request).Wait();
            
            // perform attack
            AbstractServiceHandler service = new XssAttackGetMethodInputs();
            service.StartAttack(xssPage, client);
            Console.Read();
        }
    }
}
