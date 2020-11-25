using System;
using System.Net.Http;
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
            // login to bWAPP
            /*
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
            Client.Post(request).Wait();
            */
            var login = new LoginHelper();
            login.tryLogin(Client);

            //perform attack
            AbstractServiceHandler service = new XssAttackGetMethodInputs();
            service.StartAttack(xssPage, Client);
            Console.Read();
        }
    }
}
