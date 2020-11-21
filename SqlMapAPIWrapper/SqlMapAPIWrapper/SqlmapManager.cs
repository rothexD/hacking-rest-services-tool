using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlMapAPIWrapper.Entity;

namespace SqlMapAPIWrapper
{
    public class SqlmapManager : IDisposable
    {
        private SqlmapSession _session = null;

        public SqlmapManager(SqlmapSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            _session = session;
        }

        public string NewTask()
        {
            JToken tok = JObject.Parse(_session.ExecuteGet("/task/new"));
            return tok.SelectToken("taskid").ToString();
        }

        public bool DeleteTask(string taskid)
        {
            JToken tok = JObject.Parse(_session.ExecuteGet("/task/" + taskid + "/delete"));
            return (bool) tok.SelectToken("success");
        }

        public Dictionary<string, object> GetOptions(string taskid)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            JObject tok = JObject.Parse(_session.ExecuteGet("/option/" + taskid + "/list"));
            tok = tok["options"] as JObject;
            foreach (var pair in tok)
                options.Add(pair.Key, pair.Value);
            return options;
        }

        public bool StartTask(string taskID, Dictionary<string, object> opts)
        {
            string json = JsonConvert.SerializeObject(opts);
            JToken tok = JObject.Parse(_session.ExecutePost("/scan/" + taskID + "/start", json));
            return (bool) tok.SelectToken("success");
        }

        public SqlmapStatus GetScanStatus(string taskid)
        {
            JObject tok = JObject.Parse(_session.ExecuteGet("/scan/" + taskid + "/status"));
            SqlmapStatus stat = new SqlmapStatus();
            stat.Status = (string) tok["status"];
            if (tok["returncode"].Type != JTokenType.Null)
                stat.ReturnCode = (int) tok["returncode"];
            return stat;
        }

        public List<SqlmapLogItem> GetLog(string taskid)
        {
            JObject tok = JObject.Parse(_session.ExecuteGet("/scan/" + taskid + "/log"));
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


        public void Dispose()
        {
            _session.Dispose();
            _session = null;
        }
    }
}