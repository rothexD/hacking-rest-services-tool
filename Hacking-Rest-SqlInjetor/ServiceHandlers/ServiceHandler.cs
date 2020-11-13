using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using Hacking_Rest_SqlInjetor.DatabaseInformations;
using System.Text.RegularExpressions;
using Hacking_Rest_SqlInjetor.FormDatas;

namespace Hacking_Rest_SqlInjetor.ServiceHandlers
{
    public class ServiceHandler : IServiceHandler
    {
        protected HttpClient Client { get; set; }
        public IDatabaseInformation Database { get; private set; }
        public ServiceHandler(HttpClient client,string targetUri){
            this.Client = client;
            this.Database = new DatabaseInformation();
            Database.TargetUri = targetUri;
        }
        public void StartAttack()
        {

        }
        public void GetFormDataIntoDatabaseInformation(string fakeresponse)
        {
            string response = "";
            try
            {
                response = fakeresponse;//await Client.GetStringAsync(Database.TargetUri);
            }
            catch
            {
                return;
            }
            
            var matches = Regex.Matches(response, "(<form.*?>).*?</form>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Console.WriteLine($"FOUND {matches.Count} in first regex");
            foreach(Match v_result in matches)
            {
                Console.WriteLine(v_result.ToString());
                FormData FormdataInformation = new FormData();
                //----------------------------------
                // Matches Action and Method
                Console.WriteLine($"trying to match: {v_result.Groups[1].ToString()}");
                var Action = Regex.Match(v_result.Groups[1].ToString(), ".*?action=\"(.*?)\".*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var Method = Regex.Match(v_result.Groups[1].ToString(), ".*?method=\"(.*?)\".*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (Action.Success)
                {
                    Console.WriteLine("Action was success");
                    FormdataInformation.Action = Action.Groups[1].ToString();
                }
                else
                {
                    continue;
                }
                if (Method.Success)
                {
                    Console.WriteLine("Method was success");
                    FormdataInformation.Method = Method.Groups[1].ToString();
                }
                else
                {
                    FormdataInformation.Method = "GET";
                }
                //-------------------------------------------------
                // Matches all Inputs
                var InputNames = Regex.Matches(v_result.ToString(), "<input.*?name=\"(.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach(Match v_result_InputNames in InputNames)
                {
                    Console.WriteLine($"adding: {v_result_InputNames.Groups[1].ToString()}");
                    FormdataInformation.Inputs.Add(v_result_InputNames.Groups[1].ToString());
                }





                Console.WriteLine($"adding to list");
                Database.FormDataList.Add(FormdataInformation);
            }
        }
    }
}
