using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;

namespace eveCustomLauncher
{
    class Program
    {
        static Log log;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                log = new Log();
                EveLauncher launcher = new EveLauncher();
                Settings.Init();

                if (!Settings.CheckIfECLPOpenHandlerExists())
                    Settings.SetECLPFileOpenHandler();

                string username = string.Empty;
                string password = string.Empty;
                string settingsProfile = string.Empty;

                if (args.Length > 0)
                {
                    if (args[0].ToLower().Trim().StartsWith("/profile:"))
                    {
                        string eclpFileName = args[0].Substring(9);
                        log.WriteLine("Using profile {0}", eclpFileName);
                        DPAPI dpapi = new DPAPI(eclpFileName);
                        username = dpapi.GetUserName();
                        settingsProfile = dpapi.GetSettingsProfile();
                        password = dpapi.GetPassword();
                        log.WriteLine("Username={0}, settingsProfile={1}, password length={2}", username, settingsProfile, password.Length.ToString());
                    }

                    

                    string ssoToken = launcher.GetSSO(username, password);
                    log.WriteLine("\nEverything is OK, starting EVE client...");
                    launcher.RunEVE(ssoToken, settingsProfile);
                }
                else
                {
                    Application.EnableVisualStyles();
                    frmMain form = new frmMain(launcher);
                    form.Text = ((AssemblyTitleAttribute)(Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute)))).Title;
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                log.WriteLine("Exception:");
                log.WriteLine(ex.Message, false);
                log.WriteLine(ex.StackTrace, false);
                new ErrorForm(ex).ShowDialog();
            }
        } 
    }
}
