using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Hacking_REST_Services.Form;
using Hacking_REST_Services.WebClient;
using Hacking_REST_Services.Helpers;
using SqlMapAPIWrapperLib;

namespace Hacking_REST_Services.ServiceHandlers.AttackHandlers
{
    internal class SqlInjection : AbstractServiceHandler
    {
        private readonly SqlMapApiWrapper _wrapper;
        public SqlInjection(string sqlMapApiAddress,int port)
        {
            _wrapper = new SqlMapApiWrapper(sqlMapApiAddress,port);
        }

        public override void StartAttack(string targetUri, ICustomHttpClient client)
        {
            var cookies = client.ClientHandler.CookieContainer.GetCookieHeader(new Uri(targetUri));
            Console.WriteLine(_wrapper.IsSqlinjectable(targetUri, cookies).Result
                ? $"{targetUri} : SqlInjectable"
                : $"{targetUri} : Not SqlInjectable");
        }

        public void StartAttackPost(string postData)
        {
            Console.WriteLine(_wrapper.IsSqlinjectable(postData).Result ? "SqlInjectable" : "Not SqlInjectable");
        }
        
    }
}
