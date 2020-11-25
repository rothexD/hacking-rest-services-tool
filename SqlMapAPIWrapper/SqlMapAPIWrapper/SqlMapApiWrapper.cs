using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapper.Entity;

namespace SqlMapAPIWrapper
{
    public class SqlMapApiWrapper
    {
        private string _host;
        private int _port;
        private TimeSpan waitingTime = new TimeSpan(0,0,1);
        public SqlMapApiWrapper(string SqlMapApiAdress, int port=8775)
        {
            _host = SqlMapApiAdress;
            _port = port;
        }

        public Database GetDatabaseType(string url ,string sessionCookie="")
        {
            using (SqlmapSession session = new SqlmapSession(_host,_port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = manager.NewTask();
                    Dictionary<string, object> options = manager.GetOptions(taskid);
                    options["url"] = url;
                    options["cookie"] = sessionCookie;
                    options["flushSession"] = true;
                    options["getBanner"] = true;
                    
                    
                    manager.StartTask(taskid, options);
                    SqlmapStatus status = manager.GetScanStatus(taskid);
                    while (status.Status != "terminated")
                    {
                        System.Threading.Thread.Sleep(waitingTime);
                        status = manager.GetScanStatus(taskid);
                    }

                    var data = manager.GetData(taskid);

                    if (data == null)
                        return null;

                    if (data.Data.Count < 2)
                        return null;
                    
                    Database retVal = new Database();
                    
                    var temp = (data.Data[1].Value as JArray)[0] as JObject;
                    retVal.name = temp?.GetValue("dbms")?.ToString();
                    retVal.version = temp?.GetValue("dbms_version")?.ToString();
                    
                    manager.DeleteTask(taskid);

                    if (string.IsNullOrWhiteSpace(retVal.name))
                        return null;
                    
                    return retVal;
                }
            }
        }

        public bool IsSqlinjectable(string url, string sessionCookie = "")
        {
            return !string.IsNullOrWhiteSpace(GetDatabaseType(url, sessionCookie).name);
        }

        public List<string> GetDatabases(string url, string sessionCookie = "")
        {
            List<string> ret = null;
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = manager.NewTask();
                    Dictionary<string, object> options = manager.GetOptions(taskid);
                    options["url"] = url;
                    options["cookie"] = sessionCookie;
                    options["getDbs"] = true;
                    options["excludeSysDbs"] = true;

                    manager.StartTask(taskid, options);
                    SqlmapStatus status = manager.GetScanStatus(taskid);
                    while (status.Status != "terminated")
                    {
                        System.Threading.Thread.Sleep(waitingTime);
                        status = manager.GetScanStatus(taskid);
                    }

                    var data = manager.GetData(taskid);

                    if (data != null && data.Data.Count > 0)
                    {
                        if (!data.Data[^1].IsJObject) //last index
                        {
                            var temp = data.Data[^1].Value as JArray;
                            ret = temp.ToObject <List<string>>();
                        }
                    }
                    manager.DeleteTask(taskid);
                }
                return ret;
            }
        }
    }
}