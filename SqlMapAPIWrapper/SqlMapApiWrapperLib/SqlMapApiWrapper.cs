using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapperLib.Entity;

namespace SqlMapAPIWrapperLib
{
    public class SqlMapApiWrapper 
    {
        
        private string _host;
        private int _port;
        private TimeSpan waitingTime = new TimeSpan(0, 0, 3);

        public SqlMapApiWrapper(string sqlMapApiAddress, int port = 8775)
        {
            _host = sqlMapApiAddress;
            _port = port;
        }

        public async Task<Database> GetDatabaseType(string url, string sessionCookie)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = "";
                    try
                    {
                        taskid = await manager.NewTask();
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        options["url"] = url;
                        options["cookie"] = sessionCookie;
                        options["flushSession"] = true;
                        options["getBanner"] = true;


                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckForGetDatabaseType(data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<Database> GetDatabaseType(string postData)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = "", filename = "";
                    try
                    {
                        taskid = await manager.NewTask();
                        Dictionary<string, object> options = await manager.GetOptions(taskid);
                        filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                        
                        if (!CreateFileFromData(filename, postData))
                            return null;

                        options["requestFile"] = filename;
                        options["flushSession"] = true;
                        options["getBanner"] = true;


                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckForGetDatabaseType(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                        File.Delete(filename);
                    }
                }
            }
        }

        public async Task<bool> IsSqlinjectable(string url, string sessionCookie)
        {
            return !string.IsNullOrWhiteSpace((await GetDatabaseType(url, sessionCookie))?.Name);
        }

        public async Task<bool> IsSqlinjectable(string postData)
        {
            return !string.IsNullOrWhiteSpace((await GetDatabaseType(postData))?.Name);
        }

        public async Task<List<string>> GetDatabases(string url, string sessionCookie)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = await manager.NewTask();

                    try
                    {
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        options["url"] = url;
                        options["cookie"] = sessionCookie;
                        options["getDbs"] = true;
                        options["excludeSysDbs"] = true;

                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckForGetDatabases(data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<List<string>> GetDatabases(string postData)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = await manager.NewTask();
                    string filename = "";

                    try
                    {
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                        
                        if (!CreateFileFromData(filename, postData))
                            return null;

                        options["requestFile"] = filename;
                        options["getDbs"] = true;
                        options["excludeSysDbs"] = true;

                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckForGetDatabases(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<List<string>> GetDatabaseTables(string url, string sessionCookie, string db)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = await manager.NewTask();

                    try
                    {
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        options["url"] = url;
                        options["cookie"] = sessionCookie;
                        options["getTables"] = true;
                        options["excludeSysDbs"] = true;
                        options["db"] = db;

                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckDataForGetDatabaseTables(data, db);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<List<string>> GetDatabaseTables(string postData, string db)
        {
            {
                using (SqlmapSession session = new SqlmapSession(_host, _port))
                {
                    using (SqlmapManager manager = new SqlmapManager(session))
                    {
                        string taskid = await manager.NewTask();
                        string filename = "";

                        try
                        {
                            Dictionary<string, object> options = await manager.GetOptions(taskid);

                            filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["getTables"] = true;
                            options["excludeSysDbs"] = true;
                            options["db"] = db;


                            if (!(await manager.StartTask(taskid, options)))
                                return null;


                            await CheckForStatus(manager, taskid);

                            var data = await manager.GetData(taskid);
                            return CheckDataForGetDatabaseTables(data, db);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            await manager.DeleteTask(taskid);
                        }
                    }
                }
            }
        }

        public async Task<Dictionary<string, List<string>>> GetTableContentFromDatabase(string url, string sessionCookie,
            string table, string db)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = await manager.NewTask();

                    try
                    {
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        options["url"] = url;
                        options["cookie"] = sessionCookie;
                        options["dumpTable"] = true;
                        options["excludeSysDbs"] = true;
                        options["db"] = db;
                        options["tbl"] = table;

                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);
                        return CheckDataForGetTableContentFromDatabase(data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<Dictionary<string, List<string>>> GetTableContentFromDatabase(string postData, string table, string db)
        {
            {
                using (SqlmapSession session = new SqlmapSession(_host, _port))
                {
                    using (SqlmapManager manager = new SqlmapManager(session))
                    {
                        string taskid = await manager.NewTask();
                        string filename = "";

                        try
                        {
                            Dictionary<string, object> options = await manager.GetOptions(taskid);

                            filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["dumpTable"] = true;
                            options["excludeSysDbs"] = true;
                            options["db"] = db;
                            options["tbl"] = table;


                            if (!(await manager.StartTask(taskid, options)))
                                return null;


                            await CheckForStatus(manager, taskid);

                            var data = await manager.GetData(taskid);
                            return CheckDataForGetTableContentFromDatabase(data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            await manager.DeleteTask(taskid);
                        }
                    }
                }
            }
        }

        public async Task<Dictionary<string, string>> GetDatabasePasswords(string url, string sessionCookie)
        {
            using (SqlmapSession session = new SqlmapSession(_host, _port))
            {
                using (SqlmapManager manager = new SqlmapManager(session))
                {
                    string taskid = await manager.NewTask();

                    try
                    {
                        Dictionary<string, object> options = await manager.GetOptions(taskid);

                        options["url"] = url;
                        options["cookie"] = sessionCookie;
                        options["getPasswordHashes"] = true;

                        if (!(await manager.StartTask(taskid, options)))
                            return null;

                        await CheckForStatus(manager, taskid);

                        var data = await manager.GetData(taskid);

                        return CheckDataForGetDatabasePasswords(data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        await manager.DeleteTask(taskid);
                    }
                }
            }
        }

        public async Task<Dictionary<string, string>> GetDatabasePasswords(string postData)
        {
            {
                using (SqlmapSession session = new SqlmapSession(_host, _port))
                {
                    using (SqlmapManager manager = new SqlmapManager(session))
                    {
                        string taskid = await manager.NewTask();
                        string filename = "";

                        try
                        {
                            Dictionary<string, object> options = await manager.GetOptions(taskid);

                            filename = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid()}";
                            
                            if (!CreateFileFromData(filename, postData))
                                return null;

                            options["requestFile"] = filename;
                            options["getPasswordHashes"] = true;


                            if (!(await manager.StartTask(taskid, options)))
                                return null;

                            await CheckForStatus(manager, taskid);

                            var data = await manager.GetData(taskid);

                            return CheckDataForGetDatabasePasswords(data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            await manager.DeleteTask(taskid);
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
                retVal.Name = temp?.GetValue("dbms")?.ToString();
                retVal.Version = temp?.GetValue("dbms_version")?.ToString();

                if (string.IsNullOrWhiteSpace(retVal.Name))
                    return null;
                
                return retVal;
            }
            return null;
        }

        protected async Task CheckForStatus(SqlmapManager manager ,string taskid)
        {
            SqlmapStatus status = await manager.GetScanStatus(taskid);
            while (status.Status != "terminated" && status.Status != "not running!")
            {
                System.Threading.Thread.Sleep(waitingTime);
                status = await manager.GetScanStatus(taskid);
            }
        }
    } 
}