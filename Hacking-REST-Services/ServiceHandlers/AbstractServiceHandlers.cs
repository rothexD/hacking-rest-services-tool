using System;
using System.Collections.Generic;
using System.Linq;
using Hacking_Rest_SqlInjetor.Form;
using Hacking_Rest_SqlInjetor.WebClient;
using HtmlAgilityPack;
using Hacking_REST_Services.Helpers;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers
{
    public abstract class AbstractServiceHandler : FormDataParser
    {
        public abstract void StartAttack(string targetUri, ICustomHttpClient client);     
    }
}
