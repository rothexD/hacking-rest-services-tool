using System;
using System.IO;
using System.Net;
using System.Text;

namespace SqlMapAPIWrapperLib
{
    public class SqlmapSession : IDisposable
    {
        private string _host = string.Empty;
        private int _port = 8775; //default port
        public SqlmapSession(string host, int port = 8775)
        {
            _host = host;
            _port = port;
        }
        public string ExecuteGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://" + _host + ":" + _port + url);
            Console.WriteLine(url);
            req.Method = "GET";
            string resp = string.Empty;
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(stream))
                {
                    resp = rdr.ReadToEnd();
                }
            }
            return resp;
        }
        public string ExecutePost(string url, string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://"+_host+":"+_port+url);

            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = buffer.Length;

            using (Stream stream = req.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }

            string resp = string.Empty;

            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(stream))
                {
                    resp = rdr.ReadToEnd();
                }
            }
            return resp;

        }
        public void Dispose()
        {
            _host = null;
        }
    }
}