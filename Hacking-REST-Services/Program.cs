using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Hacking_REST_Services.ServiceHandlers;
using Hacking_REST_Services.ServiceHandlers.AttackHandlers;
using Hacking_REST_Services.WebClient;
using Hacking_REST_Services.Helpers;

namespace Hacking_REST_Services
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
            /*
            var loginFields = new Dictionary<string, string>
            {
                {"login", "bee"},
                {"password", "bug"},
                {"security_level", "0"},
                {"form", "submit"}
            };

            var loginRequest = new HttpContext(HttpMethod.Post, loginPage, loginFields);
            loginRequest.BuildRequest();
            */


            ICustomHttpClient client = new CustomHttpClient();           
            var serviceDirectory = new ServiceDirectory();

            // XSS GET Parameter Injection
            serviceDirectory.AddServiceCall("XSS-INPUT", (xssGetUrl, httpClient) =>
            {
                AbstractServiceHandler service = new XSS_Attack_Inputs();
                service.StartAttack(xssGetUrl, httpClient);
            });

            serviceDirectory.AddServiceCall("XSS-SELECT", (xssGetUrl, httpClient) =>
            {
                AbstractServiceHandler service = new XSS_Attack_Selects();
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

            string input = "\0";
            while (true)
            {

                Console.WriteLine("Please specify a mode\n" +
                                      " + 0 = Login\n" +
                                      " + 1 = Clear Cookies\n" +
                                      " + 2 = XSS-input\n" +
                                      " + 3 = XSS-Selects\n" +
                                      " + 4 = SQLINJECT\n" +
                                      " + 5 = XSS-byXsstrike\n");

                input = Console.ReadLine();
                input = input.Trim();
                switch (input)
                {
                    case "0":
                        new LoginHelper().tryLogin(client);
                        break;
                    case "1":
                        client.ClientHandler.CookieContainer = new System.Net.CookieContainer();
                        break;
                    case "2":
                        Console.WriteLine("Enter the targetUri");
                        Uri targeturi;
                        try
                        {
                            targeturi = new Uri(Console.ReadLine());
                        }
                        catch
                        {
                            break;
                        }
                        Console.WriteLine(targeturi.AbsoluteUri);
                        serviceDirectory.RunTest("XSS-INPUT", targeturi.AbsoluteUri, client);
                        break;
                    case "3":
                        Console.WriteLine("Enter the targetUri");
                        try
                        {
                            targeturi = new Uri(Console.ReadLine());
                        }
                        catch
                        {
                            break;
                        }
                        serviceDirectory.RunTest("XSS-SELECT", targeturi.AbsoluteUri, client);
                        break;
                    case "4":
                        //sql injection get
                        Console.WriteLine("Enter the targetUri");
                        try
                        {
                            targeturi = new Uri(Console.ReadLine());
                        }
                        catch
                        {
                            break;
                        }
                        var openSite = client.Get(targeturi.AbsoluteUri);
                        var document = CustomHttpClient.GetHtmlDocument(openSite);
                        var formsOfHtmlDocument = FormDataParser.GetFormsOfHtmlDocument(document, targeturi.AbsoluteUri);
                        var ListHttpContext = FormDataParser.BuildHttpContexts(formsOfHtmlDocument, targeturi.AbsoluteUri);

                        foreach (var item in ListHttpContext)
                        {
                            item.BuildRequest();
                            if (item.Method == HttpMethod.Get)
                            {
                                serviceDirectory.RunTest("SQL-GET", item.RequestUri.AbsoluteUri, client);
                            }
                            else
                            {
                                serviceDirectory.RunTest("SQL-POST", item.GetRequestAsHttpString(client.ClientHandler.CookieContainer.GetCookieHeader(targeturi), null), client);
                            }
                        }
                        break;
                    case "5":
                        Console.WriteLine("Enter the targetUri");
                        string attackXSSinput = Console.ReadLine();
                        new XSSby_xsssniper().StartAttack(attackXSSinput, client);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
/*
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
 */
