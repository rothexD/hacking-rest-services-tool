using System;
using System.Collections.Generic;
using System.Linq;
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
            const string loginPage = "http://localhost:30000/login.php";
            const string xssPage = "http://localhost:30000/xss_get.php";
            const string SqlApiAddress = "127.0.0.1";
            const int SqlApiPort = 8775;            
            
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
            
            serviceDirectory.AddServiceCall("SQL-GET", (sqlGetUrl, httpClient) =>
            {
                new SqlInjection(SqlApiAddress,SqlApiPort).StartAttack(sqlGetUrl,httpClient);
            });
            
            serviceDirectory.AddServiceCall("SQL-POST", (postData, httpClient) =>
            {
                new SqlInjection(SqlApiAddress,SqlApiPort).StartAttackPost(postData);
            });

            char input = '\0';

            while (input != '0' && input != '1')
            {
                Console.WriteLine("Please specify a mode\n" +
                                  " + 0 = Automatic\n" +
                                  " + 1 = Manual");
                do
                {
                    input = (char) Console.Read();
                } while (char.IsWhiteSpace(input));
            }
            
            switch (input)
            {
                case '0':
                    serviceDirectory.RunAllTests(xssPage, client);
                    break;
                
                case '1':
                    var serviceNamesEnumerable = serviceDirectory.GetServiceNames();
                    var serviceNames = serviceNamesEnumerable.ToList();
                    Console.WriteLine("\nYou chose the manual mode ...");
                    string serviceInput;

                    while (true)
                    {
                        foreach (string name in serviceNames)
                        {
                            Console.WriteLine(" - " + name);
                        }
                        
                        Console.WriteLine("\nPlease select a service: ");
                        do
                        {
                            serviceInput = Console.ReadLine();
                        } while (string.IsNullOrWhiteSpace(serviceInput));

                        if (serviceNames.Contains(serviceInput))
                        {
                            break;
                        }
                    }
                    
                    serviceDirectory.RunTest(serviceInput, xssPage, client);
                    break;
            }
        }
    }
}
