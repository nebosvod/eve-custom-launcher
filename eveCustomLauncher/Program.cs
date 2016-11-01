using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace eveCustomLauncher
{
    class Program
    {
        const string EVE_DOMAIN_LOGIN = "login.eveonline.com";

        //Login urls
        static string[] urls = new string[]
        {
            "https://login.eveonline.com/Account/LogOn?ReturnUrl=%2Foauth%2Fauthorize%2F%3Fclient_id%3DeveLauncherTQ%26lang%3Den%26response_type%3Dcode%26redirect_uri%3Dhttps%3A%2F%2Flogin.eveonline.com%2Flauncher%3Fclient_id%3DeveLauncherTQ%26scope%3DeveClientToken%2520user",
            "https://login.eveonline.com/oauth/authorize/?client_id=eveLauncherTQ&lang=en&response_type=code&redirect_uri=https://login.eveonline.com/launcher?client_id=eveLauncherTQ&scope=eveClientToken%20user",
            "https://slinger.eveonline.com/launcher/verify",
            "https://slinger.eveonline.com/launcher/refresh",
            "https://login.eveonline.com/launcher/token"
        };

        static string exeName = "exefile.exe";
        static string exeParams = "/noconsole /server:tranquility /ssoToken={0} /settingsprofile={1} \"\"";

        //Request params
        static string rq1AuthString = "UserName={0}&Password={1}";
        static string rq3rq4Token = "Token={0}";
        static string rq5AccessToken = "?accesstoken={0}";

        //Responses
        static string cookieASPXAUTH = string.Empty;
        static string cookieUserNames = string.Empty;
        static string rq2Code = string.Empty;
        static string rq3RefreshToken = string.Empty;
        static string rq4AccessToken = string.Empty;
        static string rq5ssoToken = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists("exefile.exe"))
                    throw new Exception("exefile.exe not found");

                //Ask for user data
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                ConsoleColor fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                string password = Console.ReadLine();
                Console.ForegroundColor = fore;
                string settingsProfile = AskForSettingsProfile();

                //REQUEST 1
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urls[0]);
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] rq1AuthBytes = Encoding.ASCII.GetBytes(string.Format(rq1AuthString, username, password));
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(rq1AuthBytes, 0, rq1AuthBytes.Length);
                    stream.Flush();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                ParseCookies(response.Headers["Set-Cookie"]);
                if (cookieASPXAUTH == string.Empty)
                    throw new Exception("ASPXAUTH was empty (probably incorrect username or password)");
                Console.WriteLine("Request 1\n.ASPXAUTH={0}", cookieASPXAUTH);
                
                //REQUEST 2
                request = (HttpWebRequest)WebRequest.Create(urls[1]);
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "GET";
                request.CookieContainer = new CookieContainer(2);
                request.CookieContainer.Add(new Cookie("UserNames", cookieUserNames, "/", EVE_DOMAIN_LOGIN));
                request.CookieContainer.Add(new Cookie(".ASPXAUTH", cookieASPXAUTH, "/", EVE_DOMAIN_LOGIN));
                response = (HttpWebResponse)request.GetResponse();
                ParseResponse2(response);
                Console.WriteLine("Request 2\nCode={0}", rq2Code);

                //REQUEST 3
                request = (HttpWebRequest)WebRequest.Create(urls[2]);
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] rq3JSONBytes = Encoding.ASCII.GetBytes(string.Format(rq3rq4Token, rq2Code));
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(rq3JSONBytes, 0, rq3JSONBytes.Length);
                    stream.Flush();
                }
                response = (HttpWebResponse)request.GetResponse();
                ParseResponse3(response);
                Console.WriteLine("Request 3\nRefreshToken={0}", rq3RefreshToken);

                //REQUEST 4
                request = (HttpWebRequest)WebRequest.Create(urls[3]);
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] rq4JSONBytes = Encoding.ASCII.GetBytes(string.Format(rq3rq4Token, rq3RefreshToken));
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(rq4JSONBytes, 0, rq4JSONBytes.Length);
                    stream.Flush();
                }
                response = (HttpWebResponse)request.GetResponse();
                ParseResponse4(response);
                Console.WriteLine("Request 4\nAccessToken={0}", rq4AccessToken);

                //REQUEST 5
                request = (HttpWebRequest)WebRequest.Create(urls[4] + string.Format(rq5AccessToken, rq4AccessToken));
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();
                ParseResponse5(response);
                Console.WriteLine("Request 5\nssoToken={0}", rq5ssoToken);

                Console.WriteLine("\nEverything is OK, starting EVE client...");
                ProcessStartInfo eve = new ProcessStartInfo(exeName, string.Format(exeParams, rq5ssoToken, settingsProfile));
                Process.Start(eve);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }

        static string AskForSettingsProfile()
        {
            List<string> profiles = new List<string>();
            DirectoryInfo profileDir = new DirectoryInfo(Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\CCP\EVE");
            if (!profileDir.Exists)
                return "none";
            foreach (DirectoryInfo dir in profileDir.GetDirectories())
                foreach (DirectoryInfo innerDir in dir.GetDirectories())
                    if (innerDir.Name.Contains("settings"))
                        profiles.Add(dir.Name);
            if(profiles.Count == 0)
                return "none";
            if(profiles.Count == 1)
                return profiles[0];

            Console.WriteLine("Select profile:");
            for(int i = 0; i < profiles.Count; i++)
                Console.WriteLine("{0} - {1}", i, profiles[i]);

            Console.Write("> ");
            string profile = Console.ReadLine();
            int profileNumber = int.Parse(profile);
            if (profileNumber >= 0 && profileNumber < profiles.Count)
                return profiles[profileNumber];
            else throw new Exception("Invalid profile number");
        }

        static void ParseResponse5(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            rq5ssoToken = responseString.Split(new string[] { "access_token=" }, StringSplitOptions.None)[1].Split('&')[0];
        }

        static void ParseResponse4(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            rq4AccessToken = responseString.Split(new string[] { "AccessToken\":\"" }, StringSplitOptions.None)[1].Split('"')[0];
        }

        static void ParseResponse3(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            rq3RefreshToken = responseString.Split(new string[] { "RefreshToken\":\"" }, StringSplitOptions.None)[1].Split('"')[0];
        }

        static void ParseResponse2(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            rq2Code = responseString.Split(new string[] { "code=" }, StringSplitOptions.None)[1].Split('"')[0];
        }

        static void ParseCookies(string cookieString)
        {
            string[] cookieStrings = cookieString.Split(',',';');
            foreach (string cookie in cookieStrings)
            {
                if (cookie.Contains('='))
                {
                    string[] cookieParts = cookie.Split('=');
                    if (cookieParts.Length == 2)
                    {
                        if(cookieParts[0].Contains("UserNames"))
                            cookieUserNames = cookieParts[1];
                        else if(cookieParts[0].Contains("ASPXAUTH"))
                            cookieASPXAUTH = cookieParts[1];
                    }
                }
            }
        }
    }
}
