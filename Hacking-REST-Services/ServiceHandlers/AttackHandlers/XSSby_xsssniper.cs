using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.Form;
using Hacking_Rest_SqlInjetor.WebClient;
using Hacking_REST_Services.Helpers;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using System.Diagnostics;
using System.IO;

namespace Hacking_REST_Services.ServiceHandlers.AttackHandlers
{
    //https://github.com/s0md3v/XSStrike/wiki/Usage
    class XSSby_xsssniper : AbstractServiceHandler
    {
        /// <summary>
        /// https://stackoverflow.com/questions/11779143/how-do-i-run-a-python-script-from-c
        /// </summary>
        /// 

        private const string PathtoPython = @"C:\Users\rothexD\AppData\Local\Programs\Python\Python37\python.exe";
        private const string PathToXSSstrike = @"C:\Users\rothexD\Documents\GitHub\hacking-rest-sqlinjectior\XSStrike\xsstrike.py";
        private const string PathToCustomPayload = @"C:\Users\rothexD\Documents\GitHub\hacking-rest-sqlinjectior\XSStrike\custompayload.txt";

        private void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = PathtoPython;
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        public override void StartAttack(string targetUri, ICustomHttpClient client)
        {

            var openSite = client.Get(targetUri);
            var document = CustomHttpClient.GetHtmlDocument(openSite);
            var formsOfHtmlDocument = FormDataParser.GetFormsOfHtmlDocument(document, targetUri);
            var ListHttpContext = FormDataParser.BuildHttpContexts(formsOfHtmlDocument, targetUri);
            Console.WriteLine(ListHttpContext.Count);

            foreach(var item in ListHttpContext)
            {
                if(item.Method == HttpMethod.Get)
                {
                    item.BuildRequest();
                    Console.WriteLine($"testing: {item.RequestUri}");
                    run_cmd(PathToXSSstrike, $"-u {item.RequestUri} --skip --headers \"{"Cookie: " + client.ClientHandler.CookieContainer.GetCookieHeader(new Uri(targetUri))}\" --file \"{PathToCustomPayload}\" ");
                    continue;
                }
                item.BuildRequest();
                Console.WriteLine($"testing: {item.RequestUri}");
                string PostdataString = item.Content.ReadAsStringAsync().Result;
                Console.WriteLine(item.Content.ReadAsStringAsync().Result);
                run_cmd(PathToXSSstrike, $"-u {item.RequestUri} --skip --headers \"{"Cookie: " + client.ClientHandler.CookieContainer.GetCookieHeader(new Uri(targetUri))}\" --file \"{PathToCustomPayload}\" --data \"{PostdataString}\"");
                continue;
            }
            Console.WriteLine(client.ClientHandler.CookieContainer.GetCookieHeader(new Uri(targetUri)));
            
        }
        //Cookie: PHPSESSID=n717vtcmqcph5107nleqntkbg1; security_level=0
        public void StartAttackPost(string postData)
        {
        }
    }
}
