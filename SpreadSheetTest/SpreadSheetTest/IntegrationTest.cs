using System;
using System.Collections.Generic;
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
        public NpgsqlConnection Connection;
        public string UserId;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            RootUrl = "http://127.0.0.1";
            Handler = new HttpClientHandler();
            Client = new HttpClient(Handler);
            Wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
            Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=scw");
        }
        
        [SetUp]
        public void Setup()
        {
        }
        public async Task DeleteTableContent()
        {
            var tableNames = await GetAllTables();
            await Connection.OpenAsync();
            var transaction = await Connection.BeginTransactionAsync();

            try
            {
                foreach (var table in tableNames)
                {
                    var sql2 = $"DELETE FROM \"{table}\"";
                    var cmd2 = new NpgsqlCommand(sql2,Connection);
                    await cmd2.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Connection.CloseAsync();
            }
        }

        private async Task<List<string>> GetAllTables()
        {
            await Connection.OpenAsync();
            List<string> tableNames = new List<string>();
            try
            {
                var sql = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                var cmd = new NpgsqlCommand(sql, Connection);
                var response = await cmd.ExecuteReaderAsync();

                while (response.Read())
                {
                    tableNames.Add(response.GetString(0));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await Connection.CloseAsync();
            }

            return tableNames;
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
        [Test]
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
        [Test]
        public async Task SqlInjectionLogin()
        {
            
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test55";
            model.Password = "test55";
            string jsonData = JsonConvert.SerializeObject(model);

            string data = string.Format(HttpRequest.RequestJson, "POST", "/api/service/login", "localhost",
                CookieFromLogin, jsonData.Length, jsonData);

            var isSqlinjectable = await Wrapper.IsSqlinjectable(data);
            Assert.That(!isSqlinjectable);
        }
        
        public async Task SetTestUserToAdmin()
        {
            await Connection.OpenAsync();
            try
            {
                    var sql = "UPDATE \"Users\" set \"Role\"= 2 where \"Name\"='test'";
                    var cmd = new NpgsqlCommand(sql,Connection);
                    await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await Connection.CloseAsync();
            }
        }
        [Test]
        public async Task GetUserList()
        {
            string url = $"{RootUrl}/api/admin/user";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(response);

            foreach (var user in users)
            {
                if (user.Name == "test")
                    UserId = user.UserId.ToString();
            }
            
            Assert.That(responseTask.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(UserId) && users.Count > 1);
        }

        [Test]
        public async Task GetUser()
        {
            string url = $"{RootUrl}/api/admin/user/{UserId}";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<User>(response);

            Assert.That(responseTask.IsSuccessStatusCode && users.Name == "test"); 
        }

        [Test]
        public async Task GetUserSqlInjection()
        {
            string url = $"{RootUrl}/api/admin/user/*";
            bool isSqlinjectable = await Wrapper.IsSqlinjectable(url, "");
            
            Assert.That(!isSqlinjectable);
        }

        [Test]
        public async Task GetEmptyUserTables()
        {
            string url = $"{RootUrl}/api/admin/user/{UserId}/table";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && string.IsNullOrWhiteSpace(response)); 
        }
        [Test]
        public async Task GetEmptyUserCollaboration()
        {
            string url = $"{RootUrl}/api/admin/user/{UserId}/collaboration";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && string.IsNullOrWhiteSpace(response)); 
        }
        
        [Test]
        public async Task GetEmptyTable()
        {
            string url = $"{RootUrl}/api/admin/table";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && string.IsNullOrWhiteSpace(response)); 
        }
        
        [Test]
        public async Task GetEmptyDataset()
        {
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && string.IsNullOrWhiteSpace(response)); 
        }
        public async Task GetEmptySheet()
        {
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && string.IsNullOrWhiteSpace(response)); 
        }
        
    }
}