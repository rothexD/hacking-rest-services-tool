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
    class JustParseNoAttack : AbstractServiceHandler
    {
        protected HttpClient Client { get; set; }
        public JustParseNoAttack(HttpClient client)
        {
            this.Client = client;
        }
        override public void StartAttack(string targetUri, ICustomHttpClient Client)
        {
        }
    }
}
