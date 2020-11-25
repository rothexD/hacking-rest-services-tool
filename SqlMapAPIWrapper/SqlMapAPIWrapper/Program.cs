using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapper.Entity;

namespace SqlMapAPIWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlMapApiWrapper wrapper = new SqlMapApiWrapper("127.0.0.1",8775);
            string targetUrl = "http://localhost/sqli_1.php?title=1";
            string sessionCookie = "PHPSESSID=ea5urs4228j6jkhp5e5jja97n5; security_level=0";
            bool injectable = wrapper.IsSqlinjectable(targetUrl, sessionCookie);
            wrapper.GetDatabases(targetUrl, sessionCookie);
            /*Console.WriteLine($"Url is injectable: {injectable}");

            if (injectable)
                Console.WriteLine($"Database used is: {wrapper.GetDatabaseType(targetUrl, sessionCookie)}"); */

        }
    }
}