using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using System.Threading.Tasks;

namespace Hacking_Rest_SqlInjetor
{
    
    class Program
    {
        static readonly string Data ="<form action=\"/sqli_1.php\" method=\"GET\"><p><label for=\"title\">Search for a movie:</label><input type=\"text\" id=\"title\" name=\"title\" size=\"25\"><button type=\"submit\" name=\"action\" value=\"search\">Search</button></p></form><form action=\"/abc.php\" method=\"POST\"><p><label for=\"title\">Search for a movie:</label><input type = \"text\" id=\"title\" name=\"keks\" size=\"25\"><button type = \"submit\" name=\"action\" value=\"search\">Search</button></p></form>";
        private static readonly HttpClient _Client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string targetUri = "localhost:55001/login.php";
         
            IServiceHandler Service = new ServiceHandler(_Client,targetUri);
            Service.GetFormDataIntoDatabaseInformation(Data);
            Console.WriteLine(Service.Database.FormDataList.Count);

            foreach(var i in Service.Database.FormDataList)
            {
                Console.WriteLine("---------------");
                Console.WriteLine(i.Action);
                Console.WriteLine(i.Method);
                foreach(var b in i.Inputs)
                {
                    Console.WriteLine(b);
                }
                Console.WriteLine("---------------");
            }
            Console.Read();
        }
    }
}
