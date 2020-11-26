using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapperLib;

namespace SqlMapAPIWrapper
{
    class Program
    {
        static void Main(string[] args)
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
Cookie: security_level=0; PHPSESSID=tvhvmktm8rndsdcpqoi6aegu12
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
Cookie: security_level=0; PHPSESSID=tvhvmktm8rndsdcpqoi6aegu12
Connection: close
";

            /* SqlMapApiWrapper wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
             string targetUrl = "http://localhost/sqli_1.php?title=test";
             string targetUrlNotWorking = "http://localhost/sqli_1.php";
             string sessionCookie = "security_level=0; PHPSESSID=tvhvmktm8rndsdcpqoi6aegu12";
             string db = "bWAPP";
             string table = "users";
             bool injectable = wrapper.IsSqlinjectable(targetUrl,sessionCookie);
             bool injectable2 = wrapper.IsSqlinjectable(data);
             //bool injectable3 = wrapper.IsSqlinjectable(targetUrlNotWorking,sessionCookie);
             //bool injectable4 = wrapper.IsSqlinjectable(dataNotWorking);
             var dbs = wrapper.GetDatabases(targetUrl, sessionCookie);
             var dbs2 = wrapper.GetDatabases(data);
              var dbs3 = wrapper.GetDatabases(targetUrlNotWorking, sessionCookie);
              var dbs4 = wrapper.GetDatabases(dataNotWorking);
              var tables = wrapper.GetDatabaseTables(targetUrl, sessionCookie, db);
              var tables2 = wrapper.GetDatabaseTables(data,db);
              var tables3 = wrapper.GetDatabaseTables(targetUrlNotWorking, sessionCookie, db);
              var tables4 = wrapper.GetDatabaseTables(dataNotWorking,db);
              var tableContent = wrapper.GetTableContentFromDatabase(targetUrl, sessionCookie, table, db);
              var tableContent2 = wrapper.GetTableContentFromDatabase(data, table, db);
              var tableContent3 = wrapper.GetTableContentFromDatabase(targetUrlNotWorking, sessionCookie, table, db);
              var tableContent4 = wrapper.GetTableContentFromDatabase(dataNotWorking, table, db);
              var dbType = wrapper.GetDatabaseType(targetUrl, sessionCookie);
              var dbType2 = wrapper.GetDatabaseType(data);
              var dbType3 = wrapper.GetDatabaseType(targetUrlNotWorking, sessionCookie);
              var dbType4 = wrapper.GetDatabaseType(dataNotWorking);
              var passwords = wrapper.GetDatabasePasswords(targetUrl, sessionCookie);
              var passwords2 = wrapper.GetDatabasePasswords(data);
              var passwords3 = wrapper.GetDatabasePasswords(targetUrlNotWorking, sessionCookie);
              var passwords4 = wrapper.GetDatabasePasswords(dataNotWorking); 
              
                          bool working = false;
                          
                          if (injectable && injectable2 && dbs.Equals(dbs2))
                              working = true;
                          
                          Console.WriteLine($"Working: {working}");*/
            int i = 0;
            while (true)
            {
                try
                {
                    i++;
                    HttpWebRequest req = (HttpWebRequest) WebRequest.Create("http://127.0.0.1:8775/task/new");
                    req.Method = "GET";
                    string resp = string.Empty;
                    var stream = req.GetResponse().GetResponseStream();

                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        resp = rdr.ReadToEnd();
                    }

                    stream.Close();
                    //Console.WriteLine(resp);
                    //System.Threading.Thread.Sleep(new TimeSpan(0,0,1));
                }
                catch (Exception e)
                {
                    Console.WriteLine(i);
                    throw ;
                }
            }
        }
    }
}