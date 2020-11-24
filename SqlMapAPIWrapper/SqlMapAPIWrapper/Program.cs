using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapper.Entity;

namespace SqlMapAPIWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlmapSession session = new SqlmapSession("127.0.0.1", 8775))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = manager.NewTask();
                    Dictionary<string, object> options = manager.GetOptions(taskid);
                    options["url"] = "http://localhost/sqli_1.php?title=1"; //args[0];
                    options["flushSession"] = true;
                    //options["cookie"] = "security_level=0; PHPSESSID=ol8tknn5midgnp2unlaf90pbu5";
                    //options["getBanner"] = true;
                    
                    
                    foreach (var pair in options)
                        Console.WriteLine("Key: " + pair.Key + "\t:: Value: " + pair.Value);
                    
                    
                    manager.StartTask(taskid, options);
                    SqlmapStatus status = manager.GetScanStatus(taskid);
                    while (status.Status != "terminated")
                    {
                        System.Threading.Thread.Sleep(new TimeSpan(0, 0, 10));
                        status = manager.GetScanStatus(taskid);
                    }

                    List<SqlmapLogItem> logItems = manager.GetLog(taskid);
                    foreach (SqlmapLogItem item in logItems)
                        Console.WriteLine(item.Message);
                    manager.DeleteTask(taskid);
                }
            }
        }
    }
}