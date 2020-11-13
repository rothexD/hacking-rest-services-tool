using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Hacking_Rest_SqlInjetor.DatabaseInformations;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.FormDatas;
using Hacking_Rest_SqlInjetor.ServiceHandlers;


namespace Hacking_Rest_SqlInjetor.ServiceHandlers.AttackHandlers
{
    class XSS_attackGetMethodAbstractServiceHandler : AbstractServiceHandler
    {
        protected HttpClient Client { get; set; }
        public XSS_attackGetMethodAbstractServiceHandler(HttpClient client)
        {
            this.Client = client;
        }
        override public void StartAttack(string targetUri)
        {
        }
        private void Logik(FormData dataFields){
            Dictionary<string, string> keyValuePairsGet = new Dictionary<string, string>();         

        }
    }
}
