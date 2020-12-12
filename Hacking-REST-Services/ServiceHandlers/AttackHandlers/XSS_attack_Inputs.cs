using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.Form;
using Hacking_Rest_SqlInjetor.WebClient;
using Hacking_REST_Services.Helpers;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers
{
    internal class XSS_Attack_Inputs : AbstractXSShelper
    {
        protected override IEnumerable<HttpContext> InjectScriptIntoEveryField(IFormData form, string targetUri)
        {
            var returnList = new List<HttpContext>();
            foreach (var input in form.InputFields)
            {
                var context = new HttpContext(form.Method.ToUpper() == "GET" ? HttpMethod.Get : HttpMethod.Post, targetUri);
                foreach (var inputField in form.InputFields)
                {
                    context.AddField(inputField.Name, inputField.Value ?? "test");
                }
                context.AddField(input.Name, $"<script>alert('X{input.Name}SS')</script>");              
                foreach (var item in form.SelectFields)
                {
                    context.AddField(item.Name, item.OptionValues[0].Length > 0 ? item.OptionValues[0] : null);
                }
                returnList.Add(context);
            }         
            return returnList;                    
        }       
    }
}
