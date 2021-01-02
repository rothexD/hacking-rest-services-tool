using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public List<TableRef> TableRefList = new List<TableRef>();
        
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            RootUrl = "http://127.0.0.1";
            Wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
            Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=scw");
            await DeleteTableContent();
        }
        
        [SetUp]
        public void Setup()
        {
            Handler = new HttpClientHandler();
            Client = new HttpClient(Handler);
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
        
        [Test , Order(1)]
        public async Task RegisterUser()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);

            await SetTestUserToAdmin();
            
            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test, Order(2)]
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
        
        [Test, Order(3)]
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
        
        [Test, Order(4)]
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
        
        [Test, Order(5)]
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
        
        [Test, Order(6)]
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
        
        [Test, Order(7)]
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
        
        [Test, Order(8)]
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
        
        [Test, Order(9)]
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
        
        [Test, Order(10)]
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
        
        [Test, Order(11)]
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
        
        [Test, Order(12)]
        public async Task LoginWithUserFromNumber1Db()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test";
            model.Password = "test";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login/db";
            var responseTask  = await Client.PostAsync(url,content);
            
            Assert.That(responseTask.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(CookieFromLogin));
        }
        
        [Test, Order(13)]
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
        
        [Test, Order(14)]
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
        
        [Test, Order(15)]
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
        [Test, Order(16)]
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
        [Test, Order(17)]
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
        [Test, Order(18)]
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
            
            Assert.That(responseTask.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(UserId) && users.Count > 0);
        }

        [Test, Order(19)]
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

        [Test, Order(20)]
        public async Task GetUserSqlInjection()
        {
            string url = $"{RootUrl}/api/admin/user/*";
            bool isSqlinjectable = await Wrapper.IsSqlinjectable(url, "");
            
            Assert.That(!isSqlinjectable);
        }

        [Test, Order(21)]
        public async Task GetEmptyUserTables()
        {
            string url = $"{RootUrl}/api/admin/user/{UserId}/table";
            Client.DefaultRequestHeaders.Add("Cookie", CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && response.Length < 3); 
        }
        [Test, Order(22)]
        public async Task GetEmptyUserCollaboration()
        {
            string url = $"{RootUrl}/api/admin/user/{UserId}/collaboration";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && response.Length < 3); 
        }
        
        [Test, Order(23)]
        public async Task GetEmptyTable()
        {
            string url = $"{RootUrl}/api/admin/table";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && response.Length < 3); 
        }
        
        [Test, Order(24)]
        public async Task GetEmptyDataset()
        {
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && response.Length < 3); 
        }
        [Test, Order(25)]
        public async Task GetEmptySheet()
        {
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && response.Length < 3); 
        }
        
        [Test, Order(26)]
        public async Task CreateSheet()
        {
            CreateSheetModel model = new CreateSheetModel();
            model.DisplayName = "TestSheet";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("cookie",CookieFromLogin);
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test, Order(27)]
        public async Task CreateSheetDuplicate()
        {
            CreateSheetModel model = new CreateSheetModel();
            model.DisplayName = "TestSheet";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("cookie",CookieFromLogin);
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test, Order(28)]
        public async Task GetTestSheets()
        {
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<List<TableRef>>(response);
            TableRefList.AddRange(list);
            
            Assert.That(responseTask.IsSuccessStatusCode && response.Length > 10); 
        }
        
        [Test, Order(29)]
        public async Task CreateDataSet()
        {
            CreateDataSetModel model = new CreateDataSetModel();
            model.DisplayName = "TestSheet";
            model.Columns = new CreateDataSetModel.ColumnDefinition[2];
            model.Columns[0] = 
                new CreateDataSetModel.ColumnDefinition(){Name = "TestInteger",Type = ColumnType.Integer,Nullable = false};
            model.Columns[1] = 
                new CreateDataSetModel.ColumnDefinition(){Name = "TestString",Type = ColumnType.String,Nullable = true};
            
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("cookie",CookieFromLogin);
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test, Order(30)]
        public async Task CreateDuplicatedDataSet()
        {
            CreateDataSetModel model = new CreateDataSetModel();
            model.DisplayName = "TestSheet";
            model.Columns = new CreateDataSetModel.ColumnDefinition[2];
            model.Columns[0] = 
                new CreateDataSetModel.ColumnDefinition(){Name = "TestInteger",Type = ColumnType.Integer,Nullable = false};
            model.Columns[1] = 
                new CreateDataSetModel.ColumnDefinition(){Name = "TestString",Type = ColumnType.String,Nullable = true};
            
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("cookie",CookieFromLogin);
            var responseTask  = await Client.PostAsync(url,content);
            var response = await responseTask.Content.ReadAsStringAsync();

            
            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
                
        [Test, Order(31)]
        public async Task GetCreatedDataset()
        {
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<List<TableRef>>(response);
            TableRefList.AddRange(list);
            
            Assert.That(responseTask.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(response)); 
        }
        [Test, Order(32)]
        public async Task GetCreatedTable()
        {
            string url = $"{RootUrl}/api/admin/table";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(responseTask.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(response)); 
        }
        
        [Test, Order(33)]
        public async Task GetTableNotLoggedIn()
        {
            string url = $"{RootUrl}/api/admin/table";
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not logged in")); 
        }
        
        [Test, Order(34)]
        public async Task GetSheetNotLoggedIn()
        {
            string url = $"{RootUrl}/api/admin/sheet";
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not logged in")); 
        }
        
        [Test, Order(35)]
        public async Task GetdatasetNotLoggedIn()
        {
            string url = $"{RootUrl}/api/admin/dataset";
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not logged in")); 
        }
        
        [Test, Order(36)]
        public async Task GetUserNotLoggedIn()
        {
            string url = $"{RootUrl}/api/admin/user";
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not logged in")); 
        }
        
        [Test, Order(37)]
        public async Task RegisterUser2()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test2";
            model.Password = "test2";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/register";
            var responseTask  = await Client.PostAsync(url,content);

            Assert.That(responseTask.IsSuccessStatusCode);
        }
        
        [Test, Order(38)]
        public async Task LoginWithUserFromTestNumber37()
        {
            AuthenticationModel model = new AuthenticationModel();
            model.Username = "test2";
            model.Password = "test2";
            string jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            
            string url = $"{RootUrl}/api/service/login";
            var responseTask  = await Client.PostAsync(url,content);

            CookieFromLogin = Handler.CookieContainer.GetCookieHeader(new Uri(RootUrl));
            
            Assert.That(responseTask.IsSuccessStatusCode && !String.IsNullOrWhiteSpace(CookieFromLogin));
        }
        
        [Test, Order(39)]
        public async Task GetTableNotAuthorized()
        {
            string url = $"{RootUrl}/api/admin/table";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not allowed")); 
        }
        
        [Test, Order(40)]
        public async Task GetSheetNotAuthorized()
        {
            string url = $"{RootUrl}/api/admin/sheet";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not allowed")); 
        }
        
        [Test, Order(41)]
        public async Task GetDataSetNotAuthorized()
        {
            string url = $"{RootUrl}/api/admin/dataset";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not allowed")); 
        }
        
        [Test, Order(42)]
        public async Task GetUserNotAuthorized()
        {
            string url = $"{RootUrl}/api/admin/user";
            Client.DefaultRequestHeaders.Add("Cookie",
                CookieFromLogin);
            var responseTask  = await Client.GetAsync(url);
            var response = await responseTask.Content.ReadAsStringAsync();

            Assert.That(!responseTask.IsSuccessStatusCode && response.Contains("not allowed")); 
        }
        
        [Test, Order(43)]
        public async Task RemoveSheets()
        {
            HttpResponseMessage responseTask = null;
            foreach (var tableref in TableRefList.Where(x=>x.Type==TableType.Sheet))
            {
                string url = $"{RootUrl}/api/admin/sheet/{tableref.TableRefId}";
                Client.DefaultRequestHeaders.Add("Cookie",
                    CookieFromLogin);
                responseTask  = await Client.DeleteAsync(url);
                var response = await responseTask.Content.ReadAsStringAsync();
            }

            Assert.That(responseTask != null && responseTask.IsSuccessStatusCode); 
        }
        
        [Test, Order(44)]
        public async Task RemoveDataSheet()
        {
            HttpResponseMessage responseTask = null;
            foreach (var tableref in TableRefList.Where(x=>x.Type==TableType.DataSet))
            {
                string url = $"{RootUrl}/api/admin/dataset/{tableref.TableRefId}";
                Client.DefaultRequestHeaders.Add("Cookie",
                    CookieFromLogin);
                responseTask  = await Client.DeleteAsync(url);
                var response = await responseTask.Content.ReadAsStringAsync();
            }

            Assert.That(responseTask != null && responseTask.IsSuccessStatusCode); 
        }


    }
}