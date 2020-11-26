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

        public Database GetDatabaseType(string url, string sessionCookie)
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
                        return CheckForGetDatabaseType(data);
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
                    string taskid = "", filename = "";
                    try
                    {
                        taskid = manager.NewTask();
                        Dictionary<string, object> options = manager.GetOptions(taskid);
                        filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                        
                        if (!CreateFileFromData(filename, postData))
                            return null;

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
                        return CheckForGetDatabaseType(data);

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

        public List<string> GetDatabases(string url, string sessionCookie)
        {
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
                        return CheckForGetDatabases(data);
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
                        
                        if (!CreateFileFromData(filename, postData))
                            return null;

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
                        return CheckForGetDatabases(data);

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

        public List<string> GetDatabaseTables(string url, string sessionCookie, string db)
        {
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
                        options["getTables"] = true;
                        options["excludeSysDbs"] = true;
                        options["db"] = db;

                        manager.StartTask(taskid, options);
                        SqlmapStatus status = manager.GetScanStatus(taskid);
                        while (status.Status != "terminated")
                        {
                            System.Threading.Thread.Sleep(waitingTime);
                            status = manager.GetScanStatus(taskid);
                        }

                        var data = manager.GetData(taskid);
                        return CheckDataForGetDatabaseTables(data, db);
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

        public List<string> GetDatabaseTables(string postData, string db)
        {
            {
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
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["getTables"] = true;
                            options["excludeSysDbs"] = true;
                            options["db"] = db;


                            manager.StartTask(taskid, options);
                            SqlmapStatus status = manager.GetScanStatus(taskid);
                            while (status.Status != "terminated")
                            {
                                System.Threading.Thread.Sleep(waitingTime);
                                status = manager.GetScanStatus(taskid);
                            }

                            var data = manager.GetData(taskid);
                            return CheckDataForGetDatabaseTables(data, db);

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

        public Dictionary<string, List<string>> GetTableContentFromDatabase(string url, string sessionCookie,
            string table, string db)
        {
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
                        options["dumpTable"] = true;
                        options["excludeSysDbs"] = true;
                        options["db"] = db;
                        options["tbl"] = table;

                        manager.StartTask(taskid, options);
                        SqlmapStatus status = manager.GetScanStatus(taskid);
                        while (status.Status != "terminated")
                        {
                            System.Threading.Thread.Sleep(waitingTime);
                            status = manager.GetScanStatus(taskid);
                        }

                        var data = manager.GetData(taskid);
                        return CheckDataForGetTableContentFromDatabase(data);
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

        public Dictionary<string, List<string>> GetTableContentFromDatabase(string postData, string table, string db)
        {
            {
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
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["dumpTable"] = true;
                            options["excludeSysDbs"] = true;
                            options["db"] = db;
                            options["tbl"] = table;


                            manager.StartTask(taskid, options);
                            SqlmapStatus status = manager.GetScanStatus(taskid);
                            while (status.Status != "terminated")
                            {
                                System.Threading.Thread.Sleep(waitingTime);
                                status = manager.GetScanStatus(taskid);
                            }

                            var data = manager.GetData(taskid);
                            return CheckDataForGetTableContentFromDatabase(data);
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

        public Dictionary<string, string> GetDatabasePasswords(string url, string sessionCookie)
        {
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
                        options["getPasswordHashes"] = true;

                        manager.StartTask(taskid, options);
                        SqlmapStatus status = manager.GetScanStatus(taskid);
                        while (status.Status != "terminated")
                        {
                            System.Threading.Thread.Sleep(waitingTime);
                            status = manager.GetScanStatus(taskid);
                        }

                        var data = manager.GetData(taskid);

                        return CheckDataForGetDatabasePasswords(data);
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

        public Dictionary<string, string> GetDatabasePasswords(string postData)
        {
            {
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
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["getPasswordHashes"] = true;


                            manager.StartTask(taskid, options);
                            SqlmapStatus status = manager.GetScanStatus(taskid);
                            while (status.Status != "terminated")
                            {
                                System.Threading.Thread.Sleep(waitingTime);
                                status = manager.GetScanStatus(taskid);
                            }

                            var data = manager.GetData(taskid);

                            return CheckDataForGetDatabasePasswords(data);
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

        protected bool CreateFileFromData(string filename, string data)
        {
            try
            {
                using (FileStream fs = File.Create(filename))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        protected Dictionary<string, string> CheckDataForGetDatabasePasswords(SqlmapData data)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            
            if (data != null && data.Data.Count > 2)
            {
                if (data.Data[^1].JsonReturnType == JsonReturnType.Object) //last index
                {
                    var temp = data.Data[^1].Value as JObject;

                    if (temp == null)
                        return null;

                    foreach (var i in temp)
                    {
                        var j = i.Value as JObject;
                        var valueList = j?.GetValue("values")?.ToObject<string>();

                        ret.Add(i.Key, valueList);
                    }

                    if (ret.Count > 0)
                        return ret;
                }
            }
            return null;
        }

        protected Dictionary<string, List<string>> CheckDataForGetTableContentFromDatabase(SqlmapData data)
        {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>();
            if (data != null && data.Data.Count > 2)
            {
                if (data.Data[^1].JsonReturnType == JsonReturnType.Object) //last index
                {
                    var temp = data.Data[^1].Value as JObject;

                    if (temp == null)
                        return null;

                    foreach (var i in temp)
                    {
                        var j = i.Value as JObject;
                        var valueList = j?.GetValue("values")?.ToObject<List<string>>();

                        ret.Add(i.Key, valueList);
                    }

                    if (ret.Count > 0)
                        return ret;
                }
            }
            return null;
        }

        protected List<string> CheckDataForGetDatabaseTables(SqlmapData data,string db)
        {
            if (data != null && data.Data.Count > 2)
            {
                if (data.Data[^1].JsonReturnType == JsonReturnType.Object) //last index
                {
                    var temp = data.Data[^1].Value as JObject;
                    return temp?.GetValue(db)?.ToObject<List<string>>();
                }
            }
            return null;
        }

        protected List<string> CheckForGetDatabases(SqlmapData data)
        {
            if (data != null && data.Data.Count > 2)
            {
                if (data.Data[^1].JsonReturnType == JsonReturnType.Array) //last index
                {
                    var temp = data.Data[^1].Value as JArray;
                    return temp?.ToObject<List<string>>();
                }
            }

            return null;
        }

        protected Database CheckForGetDatabaseType(SqlmapData data)
        {
            if (data != null && data.Data.Count > 2)
            {
                Database retVal = new Database();

                var temp = (data.Data[1].Value as JArray)?[0] as JObject;
                retVal.name = temp?.GetValue("dbms")?.ToString();
                retVal.version = temp?.GetValue("dbms_version")?.ToString();

                if (string.IsNullOrWhiteSpace(retVal.name))
                    return null;
                
                return retVal;
            }
            return null;
        }
    }
}