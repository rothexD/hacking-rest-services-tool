using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapperLib.Entity;


namespace SqlMapAPIWrapperLib
{
    /// <summary>
    /// Class for handling communication with the rest api
    /// </summary>
    public class SqlmapManager : IDisposable
    {
        private SqlmapSession _session;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="session">SqlmapSession represents the sqlmap api server address</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlmapManager(SqlmapSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            _session = session;
        }

        /// <summary>
        /// Creating a new Task for further use
        /// </summary>
        /// <returns>TaskID</returns>
        public async Task<string> NewTask()
        {
            JToken tok = JObject.Parse( await _session.ExecuteGet("/task/new"));
            return tok.SelectToken("taskid")?.ToString();
        }

        /// <summary>
        /// Deletes the Task ID
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns>whether deletion was successful or not</returns>
        public async Task<bool> DeleteTask(string taskid)
        {
            JToken tok = JObject.Parse(await _session.ExecuteGet("/task/" + taskid + "/delete"));
            return (bool) tok.SelectToken("success");
        }
        /// <summary>
        /// Dictionary of Options which can be set
        /// </summary>
        /// <param name="taskid">TaskID from NewTask</param>
        /// <returns>Dictionary of options</returns>
        public async Task<Dictionary<string, object>> GetOptions(string taskid)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            JObject tok = JObject.Parse(await _session.ExecuteGet("/option/" + taskid + "/list"));
            tok = tok["options"] as JObject;
            foreach (var pair in tok)
                options.Add(pair.Key, pair.Value);
            return options;
        }

        /// <summary>
        /// starting the task with options
        /// </summary>
        /// <param name="taskID">TaskID from NewTask</param>
        /// <param name="opts">Options from GetOptions</param>
        /// <returns>whether could start the job successful</returns>
        public async Task<bool> StartTask(string taskID, Dictionary<string, object> opts)
        {
            string json = JsonConvert.SerializeObject(opts);
            JToken tok = JObject.Parse(await _session.ExecutePost("/scan/" + taskID + "/start", json));
            return (bool) tok.SelectToken("success");
        }

        /// <summary>
        /// Check if the Task is finished
        /// </summary>
        /// <param name="taskid">TaskID from NewTask</param>
        /// <returns>Returns the Status class</returns>
        public async Task<SqlmapStatus> GetScanStatus(string taskid)
        {
            JObject tok = JObject.Parse(await _session.ExecuteGet("/scan/" + taskid + "/status"));
            SqlmapStatus stat = new SqlmapStatus();
            stat.Status = (string) tok["status"];
            if (tok["returncode"].Type != JTokenType.Null)
                stat.ReturnCode = (int) tok["returncode"];
            return stat;
        }

        /// <summary>
        /// Fetching Log from TaskID
        /// </summary>
        /// <param name="taskid">TaskID from NewTask</param>
        /// <returns>List of SqlmapLogItem</returns>
        public async Task<List<SqlmapLogItem>> GetLog(string taskid)
        {
            JObject tok = JObject.Parse(await _session.ExecuteGet("/scan/" + taskid + "/log"));
            JArray items = tok["log"] as JArray;
            List<SqlmapLogItem> logItems = new List<SqlmapLogItem>();
            foreach (var item in items)
            {
                SqlmapLogItem i = new SqlmapLogItem();
                i.Message = (string) item["message"];
                i.Level = (string) item["level"];
                i.Time = (string) item["time"];
                logItems.Add(i);
            }

            return logItems;
        }

        /// <summary>
        /// Parses Results into SqlmapData
        /// Data still needs to be parsed to be outputed correctly 
        /// </summary>
        /// <param name="taskid">TaskID from NewTask</param>
        /// <returns>SqlMapData might need further parsing</returns>
        public async Task<SqlmapData> GetData(string taskid)
        {
            string json = await _session.ExecuteGet("/scan/" + taskid + "/data");
            JObject tok = JObject.Parse(json);
            JArray items = tok["data"] as JArray;
            SqlmapData data = new SqlmapData();
            foreach (var item in items)
            {
                SqlmapDataItem i = new SqlmapDataItem();
                i.Status = (int) item["status"];
                i.Type = (int) item["type"];

                switch (i.Type)
                {
                    case 0:
                    case 13:
                    case 17:
                    case 9:
                        i.Value = (object) item["value"] as JObject;
                        i.JsonReturnType = JsonReturnType.Object;
                        break;
                    case 2:
                    case 3:
                        i.Value = item["value"]?.ToString();
                        i.JsonReturnType = JsonReturnType.String;
                        break;
                    default:
                        i.Value = (object) item["value"] as JArray;
                        break;
                }

                data.Data.Add(i);
            }

            data.Success = (bool) tok["success"];

            if (data.Data.Count < 1)
                return null;
            return data;
        }


        public void Dispose()
        {
            _session.Dispose();
            _session = null;
        } 
    }
}