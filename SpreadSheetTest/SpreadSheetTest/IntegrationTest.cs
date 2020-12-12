using System;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Newtonsoft.Json;
using Npgsql;
using NUnit.Framework;
using SpreadSheetTest.Entities;
using SqlMapAPIWrapperLib;


namespace SpreadSheetTest
{
    public class Tests
    {
        public HttpClientHandler Handler;
        public HttpClient Client;
        public string CookieFromLogin="";
        public string RootUrl { get; set; }
        public SqlMapApiWrapper Wrapper;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            RootUrl = "http://127.0.0.1";
            Handler = new HttpClientHandler();
            Client = new HttpClient(Handler);
            Wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
        }
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void test()
        {
        }
        

        [Test]
        public async Task RegisterUser()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);

            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test]
        public async Task RegisterSameUserAgain()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);
            var responsebody = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && 
                        responsebody.Contains(ErrorMessages.Duplicate));
        }
        
        [Test]
        public async Task RegisterWithNoValues()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);
            var responsebody = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && 
                        responsebody.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task RegisterWithNoUsername()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "sss";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);
            var responsebody = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && 
                        responsebody.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task RegisterWithNoPassword()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "sss";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);
            var responsebody = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && 
                        responsebody.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task RegisterWithSpecialCharacter()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "!§\\\"$%%&/())s";
            model.Password = "-.,!§$%&\\\"/()=?";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);

            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test]
        public async Task LoginWithUserFromTestNumber1()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);

            CookieFromLogin = Handler.CookieContainer.GetCookieHeader(new Uri(RootUrl));
            
            Assert.That(responseTask.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(CookieFromLogin));
        }
        
        [Test]
        public async Task LoginWithNoValues()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task LoginWithNoUsername()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task LoginWithNoPassword()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task LoginWithNotExistingUser()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test55";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NotFound));
        }
        
        [Test]
        public async Task LoginWithUserFromNumber1Db()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test55";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            CookieFromLogin = Handler.CookieContainer.GetCookieHeader(new Uri(RootUrl));
            
            Assert.That(responseTask.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(CookieFromLogin));
        }
        
        [Test]
        public async Task LoginWithNoUsernameDb()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task LoginWithNoPasswordDb()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "222";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        [Test]
        public async Task LoginWithNoValuesDb()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "";
            model.Password = "";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NoValue));
        }
        
        public async Task LoginWithNotExistingUserDb()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test55";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains(ErrorMessages.NotFound));
        }
        
        public async Task SqlInjectionLogin()
        {
            
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test55";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);
            
            string data = $@"POST {RootUrl}/api/service/login HTTP/1.1
Host: localhost
Content-Length: 24
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
Origin: http://localhost
Content-Type: application/x-www-form-urlencoded
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
Sec-Fetch-Site: same-origin
Sec-Fetch-Mode: navigate
Sec-Fetch-User: ?1
Sec-Fetch-Dest: document
Referer: {RootUrl}
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9
Cookie: {CookieFromLogin}
Connection: close

{jsonData}";
            
            var isSqlinjectable = await Wrapper.IsSqlinjectable(data);
            Assert.That(!isSqlinjectable);
        }
        
    }
}