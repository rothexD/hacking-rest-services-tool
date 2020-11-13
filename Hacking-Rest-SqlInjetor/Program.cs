using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using System.Threading.Tasks;
using Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor
{
    
    class Program
    {
        static readonly string Data = "<form action=\"/sqli_1.php\" method=\"GET\"><p><label for=\"title\">Search for a movie:</label><input type=\"text\" value=\"hans\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><button type=\"submit\" name=\"action\" value=\"search\">Search</button></p></form><form action=\"/abc.php\" method=\"POST\"><p><label for=\"title\">Search for a movie:</label><input type=\"text\" id=\"title\" name=\"keks\" size=\"25\"><button type=\"submit\" name=\"action\" value=\"search\">Search</button></p></form>";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ICustomHttpClient _Client = new CustomHttpClient();
           

            const string loginPage = "http://localhost:55001/login.php";
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

            var response = _Client.Post(request).Result;
            string responseString = response.Content.ReadAsStringAsync().Result;


            AbstractServiceHandler Service = new XSS_attackGetMethod_Inputs();
            Service.StartAttack("http://localhost:55001/xss_get.php", _Client);
            Console.Read();
        }
    }
}
