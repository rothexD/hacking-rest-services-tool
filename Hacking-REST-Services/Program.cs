using System;
using System.Collections.Generic;
using System.Net.Http;
using Hacking_REST_Services.ServiceHandlers;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string loginPage = "http://localhost:55001/login.php";
            const string xssPage = "http://localhost:55001/xss_get.php";

            // login to bWAPP
            var loginFields = new Dictionary<string, string>
            {
                {"login", "bee"},
                {"password", "bug"},
                {"security_level", "0"},
                {"form", "submit"}
            };

            var loginRequest = new HttpContext(HttpMethod.Post, loginPage, loginFields);
            loginRequest.BuildRequest();
            
            ICustomHttpClient client = new CustomHttpClient();
            client.Post(loginRequest).Wait();
            
            var serviceDirectory = new ServiceDirectory();

            // XSS GET Parameter Injection
            serviceDirectory.AddServiceCall("XSS-GET", (xssGetUrl, httpClient) =>
            {
                AbstractServiceHandler service = new XssAttackGetMethodInputs();
                service.StartAttack(xssGetUrl, httpClient);
            });
        }
    }
}
