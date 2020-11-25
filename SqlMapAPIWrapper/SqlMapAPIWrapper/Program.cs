using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
Cookie: PHPSESSID=n9fj3muu2a12rm6erlip21cfc6; security_level=0
Connection: close

title=test&action=search";

            SqlMapApiWrapper wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
            string targetUrl = "http://localhost/sqli_1.php?title=1";
            string sessionCookie = "PHPSESSID=n9fj3muu2a12rm6erlip21cfc6; security_level=0";
            bool injectable = wrapper.IsSqlinjectable(targetUrl,sessionCookie);
            bool injectable2 = wrapper.IsSqlinjectable(data);
            var tables = wrapper.GetDatabases(targetUrl, sessionCookie);
            var tables2 = wrapper.GetDatabases(data);
            /*Console.WriteLine($"Url is injectable: {injectable}");

            if (injectable)
                Console.WriteLine($"Database used is: {wrapper.GetDatabaseType(targetUrl, sessionCookie)}"); */

        }
    }
}