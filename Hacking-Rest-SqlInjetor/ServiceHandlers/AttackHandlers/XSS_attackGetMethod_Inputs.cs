using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Hacking_Rest_SqlInjetor.DatabaseInformations;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.FormDatas;
using Hacking_Rest_SqlInjetor.ServiceHandlers;
using Hacking_Rest_SqlInjetor.WebClient;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers
{
    class XSS_attackGetMethod_Inputs : AbstractServiceHandler
    {
        public XSS_attackGetMethod_Inputs()
        {
        }
        override public void StartAttack(string targetUri,ICustomHttpClient Client)
        {
            var resultat = Client.Get(targetUri);
            string htmlAsString = resultat.Result.Content.ReadAsStringAsync().Result;
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlAsString);

            List<FormData> formsOfHtml = GetFormDataIntoDatabaseInformation(htmlDocument, targetUri);
            foreach(FormData singleForm in formsOfHtml)
            {
                if (singleForm.Method != "GET")
                {
                    continue;
                }
                var HttpContextes = Logik(singleForm, targetUri);
                foreach(var Context in HttpContextes)
                {
                    Context.BuildRequest();
                    //Console.WriteLine(Context.RequestUri);
                    var result = Client.Get(Context);
                    var Match = Regex.Match(result.Result.Content.ReadAsStringAsync().Result, @"<script>alert\('X(.*?)SS'\)</script>",RegexOptions.IgnoreCase|RegexOptions.Singleline);
                   //Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
                    if (Match.Success)
                    {
                        Console.WriteLine($"Possible XSS-Attack Vector found in InputField on {targetUri}: {Match.Groups[1].Value}");
                       
                    }
                }
            }
        }
        private List<HttpContext> Logik(FormData dataFields,string targeturi){
            List<HttpContext> ReturnList = new List<HttpContext>();          
            for(int i = 0; i < dataFields.InputFields.Count; i++)
            {
                HttpContext context = new HttpContext(HttpMethod.Get,targeturi);
                for (int k = 0; k < dataFields.InputFields.Count; k++)
                {
                    InputInformation Inputfield = dataFields.InputFields[k];
                    if (Inputfield.Value == null)
                    {
                        context.AddField(Inputfield.Name, "test");
                        continue;
                    }
                    context.AddField(Inputfield.Name, Inputfield.Value);                   
                }
                context.AddField("form", "submit");
                context.AddField(dataFields.InputFields[i].Name, $"<script>alert('X{dataFields.InputFields[i].Name}SS')</script>");
                ReturnList.Add(context);
            }
            return ReturnList;                    
        }       
    }
}
