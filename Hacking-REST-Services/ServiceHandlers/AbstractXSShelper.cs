using System;
using System.Collections.Generic;
using System.Linq;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using HtmlAgilityPack;
using Hacking_REST_Services.Helpers;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace Hacking_REST_Services.ServiceHandlers
{
    public abstract class AbstractXSShelper : AbstractServiceHandler
    {
        public override void StartAttack(string targetUri, ICustomHttpClient client)
        {
            Console.WriteLine($"cookiecount: {client.ClientHandler.CookieContainer.Count}");
            var openSite = client.Get(targetUri);
            var document = CustomHttpClient.GetHtmlDocument(openSite);
            var formsOfHtmlDocument = GetFormsOfHtmlDocument(document, targetUri);

            foreach (var form in formsOfHtmlDocument)
            {
                if (form.Method != "GET" && form.Method != "POST")
                {
                    continue;
                }

                var requests = InjectScriptIntoEveryField(form, targetUri);
                foreach (var request in requests)
                {
                    request.BuildRequest();

                    var performGetInjection = request.Method == HttpMethod.Get ? client.Get(request) : client.Post(request);                   
                    string injectionResponse = CustomHttpClient.GetResponseContent(performGetInjection);
                    const string scriptRegex = @"<script>alert\('X(.*?)SS'\)</script>";
                    const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                    var match = Regex.Match(injectionResponse, scriptRegex, options);

                    if (match.Success)
                    {
                        Console.WriteLine($"Possible XSS-Attack Vector found in InputField on {targetUri}: {match.Groups[1].Value}");
                    }
                }
            }
        }
        protected abstract IEnumerable<HttpContext> InjectScriptIntoEveryField(IFormData form, string targetUri);

    }
}
