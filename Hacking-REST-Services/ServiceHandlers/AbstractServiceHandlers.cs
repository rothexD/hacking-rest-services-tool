using System;
using System.Collections.Generic;
using System.Linq;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using HtmlAgilityPack;
using Hacking_REST_Services.Helpers;

namespace Hacking_REST_Services.ServiceHandlers
{
    public abstract class AbstractServiceHandler : FormDataParser
    {
        public abstract void StartAttack(string targetUri, ICustomHttpClient client);     
    }
}
