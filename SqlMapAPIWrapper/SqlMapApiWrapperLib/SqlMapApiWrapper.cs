using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapperLib.Entity;

namespace SqlMapAPIWrapperLib
{
    public class SqlMapApiWrapper
    {
        private string _host;
        private int _port;
        private TimeSpan waitingTime = new TimeSpan(0, 0, 1);

        public SqlMapApiWrapper(string SqlMapApiAdress, int port = 8775)
        {
            _host = SqlMapApiAdress;
            _port = port;
        }

        public Database GetDatabaseType(string url, string sessionCookie = "")
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = "";
                    try
                    {
                        taskid = manager.NewTask();
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

                        var temp = (data.Data[1].Value as JArray)?[0] as JObject;
                        retVal.name = temp?.GetValue("dbms")?.ToString();
                        retVal.version = temp?.GetValue("dbms_version")?.ToString();


                        if (string.IsNullOrWhiteSpace(retVal.name))
                            return null;

                        return retVal;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        manager.DeleteTask(taskid);
                    }
                    
                }
            }
        }
        
        public Database GetDatabaseType(string postData)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid="",filename="";
                    try
                    {
                        taskid = manager.NewTask();
                        Dictionary<string, object> options = manager.GetOptions(taskid);
                        filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                        using (FileStream fs = File.Create(filename))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(postData);
                            fs.Write(info, 0, info.Length);
                        }

                        options["requestFile"] = filename;
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

                        var temp = (data.Data[1].Value as JArray)?[0] as JObject;
                        retVal.name = temp?.GetValue("dbms")?.ToString();
                        retVal.version = temp?.GetValue("dbms_version")?.ToString();

                        if (string.IsNullOrWhiteSpace(retVal.name))
                            return null;

                        return retVal;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        manager.DeleteTask(taskid);
                        File.Delete(filename);
                    }
                    
                }
            }
        }

        public bool IsSqlinjectable(string url, string sessionCookie)
        {
            return !string.IsNullOrWhiteSpace(GetDatabaseType(url, sessionCookie)?.name);
        }
        
        public bool IsSqlinjectable(string postData)
        {
            return !string.IsNullOrWhiteSpace(GetDatabaseType(postData)?.name);
        }

        public List<string> GetDatabases(string url, string sessionCookie = "")
        {
            List<string> ret = null;
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = manager.NewTask();

                    try
                    {
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
                            if (data.Data[^1].JsonReturnType == JsonReturnType.Array) //last index
                            {
                                var temp = data.Data[^1].Value as JArray;
                                ret = temp?.ToObject<List<string>>();
                            }
                        }

                        return ret;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        manager.DeleteTask(taskid);
                    }
                }
            }
        }
        
        public List<string> GetDatabases(string postData)
        {
            List<string> ret = null;
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = manager.NewTask();
                    string filename = "";

                    try
                    {
                        Dictionary<string, object> options = manager.GetOptions(taskid);

                        filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                        using (FileStream fs = File.Create(filename))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(postData);
                            fs.Write(info, 0, info.Length);
                        }

                        options["requestFile"] = filename;
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
                            if (data.Data[^1].JsonReturnType == JsonReturnType.Array) //last index
                            {
                                var temp = data.Data[^1].Value as JArray;
                                ret = temp?.ToObject<List<string>>();
                            }
                        }

                        return ret;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        manager.DeleteTask(taskid);
                    }
                }
            }
        }
    }
}