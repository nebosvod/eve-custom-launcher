using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Web;

namespace eveCustomLauncher
{
    public class EveLauncher
    {
        const string EXE_NAME = "exefile.exe";
        const string EXE_PARAMS = "/noconsole /server:tranquility /ssoToken={0} /settingsprofile={1} \"\"";

        const string EVE_DOMAIN_LOGIN = "login.eveonline.com";

        //Login urls
        string[] urls = new string[]
        {
            "https://login.eveonline.com/Account/LogOn?ReturnUrl=%2Foauth%2Fauthorize%2F%3Fclient_id%3DeveLauncherTQ%26lang%3Den%26response_type%3Dcode%26redirect_uri%3Dhttps%3A%2F%2Flogin.eveonline.com%2Flauncher%3Fclient_id%3DeveLauncherTQ%26scope%3DeveClientToken%2520user",
            "https://login.eveonline.com/oauth/authorize/?client_id=eveLauncherTQ&lang=en&response_type=code&redirect_uri=https://login.eveonline.com/launcher?client_id=eveLauncherTQ&scope=eveClientToken%20user",
            "https://slinger.eveonline.com/launcher/verify",
            "https://slinger.eveonline.com/launcher/refresh",
            "https://login.eveonline.com/launcher/token"
        };

        string urlEula = @"https://login.eveonline.com/OAuth/Eula";
        string eulaParams = @"eulaHash={0}&returnUrl=https%3A%2F%2Flogin.eveonline.com%2Foauth%2Fauthorize%2F%3Fclient_id%3DeveLauncherTQ%26lang%3Den%26response_type%3Dcode%26redirect_uri%3Dhttps%3A%2F%2Flogin.eveonline.com%2Flauncher%3Fclient_id%3DeveLauncherTQ%26scope%3DeveClientToken%2520user&action=Accept";

        //Request params
        static string rq1AuthString = "UserName={0}&Password={1}";
        static string rq3rq4Token = "Token={0}";
        static string rq5AccessToken = "?accesstoken={0}";

        //Responses
        static string cookieASPXAUTH = string.Empty;
        static string cookieUserNames = string.Empty;
        static string rq2Code = string.Empty;
        string rq3RefreshToken = string.Empty;
        string rq4AccessToken = string.Empty;
        string rq5ssoToken = string.Empty;

        bool needToVerifyEmail = false;
        bool needToAcceptEula = false;
        string eulaHash = string.Empty;

        Log log;
        public EveLauncher()
        {
            log = Log.Instance;
            log.WriteLine("EveLauncherInit");
        }

        public string GetSSO(string username, string password)
        {
            string sso = GetSSOInt(username, password);
            log.WriteLine("SSO: {0}", sso);
            if (sso == "eula")
            {
                Thread.Sleep(10000);
                sso = GetSSOInt(username, password);
                if (sso == "eula")
                    throw new Exception("An error occured while accepting the EULA, please restart application and try again");
                else return sso;
            }
            else if (sso == "verify")
                throw new Exception("Email address of this account need to be verified first!");
            else return sso;
        }

        private string GetSSOInt(string username, string password)
        {
            //REQUEST 1
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urls[0]);
            request.ServicePoint.Expect100Continue = false;
            request.AllowAutoRedirect = false;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] rq1AuthBytes = Encoding.ASCII.GetBytes(string.Format(rq1AuthString, EscapeString(username), EscapeString(password)));
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(rq1AuthBytes, 0, rq1AuthBytes.Length);
                stream.Flush();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            log.WriteLine("Response 1");
            log.WriteHttpWebResponseFull(response);

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

            log.WriteLine("Response 2");
            log.WriteHttpWebResponse(response);

            ParseResponse2(response);
            if (needToVerifyEmail)
            {
                return "verify";
            }
            if (needToAcceptEula)
            {
                log.WriteLine("Need to accept eula, eulaHash={0}", eulaHash);
                //REQUEST 2.1 (EULA)
                request = (HttpWebRequest)WebRequest.Create(urlEula);
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = new CookieContainer(2);
                request.CookieContainer.Add(new Cookie("UserNames", cookieUserNames, "/", EVE_DOMAIN_LOGIN));
                request.CookieContainer.Add(new Cookie(".ASPXAUTH", cookieASPXAUTH, "/", EVE_DOMAIN_LOGIN));
                byte[] eulaBytes = Encoding.ASCII.GetBytes(string.Format(eulaParams, eulaHash));
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(eulaBytes, 0, eulaBytes.Length);
                    stream.Flush();
                }
                response = (HttpWebResponse)request.GetResponse();
                log.WriteHttpWebResponse(response);
                string responseString = GetStringFromResponse(response, string.Empty, string.Empty)[0];
                log.WriteLine(responseString, false);
                return "eula";
            }

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

            log.WriteLine("Response 3");
            log.WriteHttpWebResponse(response);

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

            log.WriteLine("Response 4");
            log.WriteHttpWebResponse(response);

            response = (HttpWebResponse)request.GetResponse();
            ParseResponse4(response);

            Console.WriteLine("Request 4\nAccessToken={0}", rq4AccessToken);

            //REQUEST 5
            request = (HttpWebRequest)WebRequest.Create(urls[4] + string.Format(rq5AccessToken, rq4AccessToken));
            request.ServicePoint.Expect100Continue = false;
            request.AllowAutoRedirect = false;
            request.Method = "GET";
            response = (HttpWebResponse)request.GetResponse();

            log.WriteLine("Response 5");
            log.WriteHttpWebResponse(response);

            ParseResponse5(response);

            Console.WriteLine("Request 5\nssoToken={0}", rq5ssoToken);
            return rq5ssoToken;
        }

        void ParseResponse5(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            log.WriteLine(responseString, false);
            rq5ssoToken = responseString.Split(new string[] { "access_token=" }, StringSplitOptions.None)[1].Split('&')[0];
        }

        void ParseResponse4(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            log.WriteLine(responseString, false);
            rq4AccessToken = responseString.Split(new string[] { "AccessToken\":\"" }, StringSplitOptions.None)[1].Split('"')[0];
        }

        void ParseResponse3(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            log.WriteLine(responseString, false);
            rq3RefreshToken = responseString.Split(new string[] { "RefreshToken\":\"" }, StringSplitOptions.None)[1].Split('"')[0];
        }

        void ParseResponse2(HttpWebResponse response)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            log.WriteLine(responseString, false);

            if (responseString.Contains("eulaHash"))
            {
                needToAcceptEula = true;
                eulaHash = GetString(responseString, "input id=\"eulaHash\" name=\"eulaHash\" type=\"hidden\" value=\"", "\"");
            }
            else if (responseString.Contains("Verification of the email address associated with your account has not been completed"))
                needToVerifyEmail = true;
            else rq2Code = responseString.Split(new string[] { "code=" }, StringSplitOptions.None)[1].Split('"')[0];
        }



        string[] GetStringFromResponse(HttpWebResponse response, string splitBefore, string splitAfter)
        {
            string responseString = string.Empty;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseString = reader.ReadToEnd();
            }
            if (splitBefore == string.Empty && splitAfter == string.Empty)
            {
                return new string[] { responseString };
            }
            string splittedString = responseString.Split(new string[] { splitBefore }, StringSplitOptions.None)[1].Split(new string[] {splitAfter},  StringSplitOptions.None)[0];
            return new string[] { responseString, splittedString };
        }

        string GetString(string source, string splitBefore, string splitAfter)
        {
            return source.Split(new string[] { splitBefore }, StringSplitOptions.None)[1].Split(new string[] { splitAfter }, StringSplitOptions.None)[0];
        }

        void ParseCookies(string cookieString)
        {
            string[] cookieStrings = cookieString.Split(',', ';');
            foreach (string cookie in cookieStrings)
            {
                if (cookie.Contains('='))
                {
                    string[] cookieParts = cookie.Split('=');
                    if (cookieParts.Length == 2)
                    {
                        if (cookieParts[0].Contains("UserNames"))
                            cookieUserNames = cookieParts[1];
                        else if (cookieParts[0].Contains("ASPXAUTH"))
                            cookieASPXAUTH = cookieParts[1];
                    }
                }
            }
        }

        

        public static string[] GetSettingsProfilesList()
        {
            Log log = Log.Instance;
            List<string> profiles = new List<string>();

            string settingsProfilePath = GetSettingsProfilePathForClient(Settings.GetEveInstallationDirectory());

            if (Directory.Exists(settingsProfilePath))
            {
                foreach (DirectoryInfo innerDir in new DirectoryInfo(settingsProfilePath).GetDirectories())
                    if (innerDir.Name.StartsWith("settings_"))
                        profiles.Add(innerDir.Name.Substring(9));
            }
            return profiles.ToArray();
        }

        public static string GetSettingsProfilePathForClient(string pathToExefile)
        {
            Log log = Log.Instance;
            DirectoryInfo profileDir = GetEVEProfileDir();

            if (!profileDir.Exists)
            {
                throw new Exception(@"CCP\EVE folder not found in local application data!");
                //TODO: Allow user to force start game client
            }
            log.WriteLine("Settings profile path: {0}", profileDir.FullName);

            string gameBasePath = new FileInfo(pathToExefile).Directory.Parent.FullName;
            gameBasePath = gameBasePath.Replace(@":\", "_");
            gameBasePath = gameBasePath.Replace('\\', '_');
            gameBasePath += "_tranquility";
            gameBasePath = Path.Combine(profileDir.FullName, gameBasePath);

            return gameBasePath;
        }

        public static void CreateSettingsProfile(string name)
        {
            DirectoryInfo settingsProfilePath = new DirectoryInfo(GetSettingsProfilePathForClient(Settings.GetEveInstallationDirectory()));
            settingsProfilePath.CreateSubdirectory(string.Format("settings_{0}", name));
        }

        public static string FindExefile()
        {
            Log log = Log.Instance;
            var cacheDirectoryFromRegistry = Registry.GetValue(@"HKEY_CURRENT_USER\Software\CCP\EVEONLINE\", "CACHEFOLDER", null);
            if (cacheDirectoryFromRegistry != null)
            {
                string pathNewClient = Path.Combine((string)cacheDirectoryFromRegistry, @"tq\bin\" + EXE_NAME);
                string pathOldClient = Path.Combine((string)cacheDirectoryFromRegistry, @"bin\" + EXE_NAME);
                if (File.Exists(pathNewClient))
                    return pathNewClient;
                if (File.Exists(pathOldClient))
                {
                    log.WriteLine("CACHEFOLDER entry pointing to an old client!");
                    return pathOldClient;
                }
                log.WriteLine("Incorrect CACHEFOLDER entry in registry!");
            }
            else log.WriteLine("No CACHEFOLDER entry in registry!");

            string cacheDirectoryFromECLPath = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName;
            string path = Path.Combine(cacheDirectoryFromECLPath, EXE_NAME);
            if (File.Exists(path))
                return path;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.Description = "Select directory which contains exefile.exe";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string userPath = Path.Combine(dialog.SelectedPath, EXE_NAME);
                return userPath;
            }
            return string.Empty;
        }

        static DirectoryInfo GetEVEProfileDir()
        {
            DirectoryInfo path;
            if (IsWinXP)
            {
                path = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Local Settings\Application Data\CCP\EVE");
            }
            else
            {
                path = new DirectoryInfo(Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\CCP\EVE");
            }
            Log.Instance.WriteLine("GetEVEProfileDir() = {0}", path.FullName);
            return path;
        }

        public void RunEVE(string ssoToken, string settingsProfile)
        {
            ProcessStartInfo eve = new ProcessStartInfo(Settings.GetEveInstallationDirectory(), string.Format(EXE_PARAMS, ssoToken, settingsProfile));
            Process.Start(eve);
        }

        static bool IsWinXP
        {
            get
            {
                return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1;
            }
        }

        public static string EscapeString(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
    }
}
