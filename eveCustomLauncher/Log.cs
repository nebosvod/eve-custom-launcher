using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace eveCustomLauncher
{
    public class Log
    {
        StreamWriter sw;
        const string format = "yyyy-MM-dd.HH-mm-ss.fffff";
        public string fileName = string.Empty;
        public Log()
        {
            fileName = "ECL." + DateTime.Now.ToString(format) + ".log";
            sw = new StreamWriter(fileName);
            sw.AutoFlush = true;
        }

        private string GetTimestamp()
        {
            return string.Format("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));
        }

        public void WriteLine(string text, bool addTimestamp)
        {
            sw.WriteLine((addTimestamp ? GetTimestamp() : string.Empty) + text);
        }

        public void WriteLine(string text)
        {
            WriteLine(text, true);
        }

        public void WriteLine(bool addTimestamp, string format, params object[] arg)
        {
            sw.WriteLine((addTimestamp ? GetTimestamp() : string.Empty) + format, arg);
        }

        public void WriteLine(string format, params object[] arg)
        {
            WriteLine(true, format, arg);
        }

        public void WriteHttpWebResponseFull(HttpWebResponse response)
        {
            WriteLine("HttpWebResponse from {0}, status {1}", response.Server, response.StatusCode);
            WriteLine("Headers: ", false);
            WebHeaderCollection headers = response.Headers;
            foreach (string header in headers.AllKeys)
            {
                WriteLine(false, "{0}: {1}", header, headers[header]);
            }

            WriteLine("Body: ", false);
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            WriteLine(responseString, false);
        }

        public void WriteHttpWebResponse(HttpWebResponse response)
        {
            WriteLine("HttpWebResponse from {0}, status {1}", response.Server, response.StatusCode);
            WriteLine("Headers: ", false);
            WebHeaderCollection headers = response.Headers;
            foreach (string header in headers.AllKeys)
            {
                WriteLine(false, "{0}: {1}", header, headers[header]);
            }

            WriteLine("Body: ", false);
        }
    }
}
