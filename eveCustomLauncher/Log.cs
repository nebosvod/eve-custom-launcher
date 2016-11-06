using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace eveCustomLauncher
{
    public class Log
    {
        public static Log Instance = null;
        StreamWriter sw;
        const string format = "yyyy-MM-dd.HH-mm-ss.fffff";
        public string fileName = string.Empty;
        public Log()
        {
            DirectoryInfo tempDir = new DirectoryInfo(Environment.GetEnvironmentVariable("TEMP"));
            fileName = "ECL." + DateTime.Now.ToString(format) + ".log";
            if (Directory.Exists(tempDir.FullName))
            {
                if(!Directory.Exists(Path.Combine(tempDir.FullName, "ECL")))
                    tempDir.CreateSubdirectory("ECL");
                fileName = Path.Combine(Path.Combine(tempDir.FullName, "ECL"), fileName);
            }
            sw = new StreamWriter(fileName);
            sw.AutoFlush = true;
            Instance = this;
            WriteLine("ECL {0} startup", Assembly.GetEntryAssembly().GetName().Version);
            WriteLine("System: {0}", Environment.OSVersion.VersionString);
        }

        public string GetText()
        {
            string copyFileName = fileName + "__1";
            File.Copy(fileName, copyFileName);
            string text = File.ReadAllText(copyFileName);
            File.Delete(copyFileName);
            return text;
        }

        private string GetTimestamp()
        {
            return string.Format("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));
        }

        public void WriteLine(bool addTimestamp, string text)
        {
            sw.WriteLine((addTimestamp ? GetTimestamp() : string.Empty) + text);
        }

        public void WriteLine(string text)
        {
            WriteLine(true, text);
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
