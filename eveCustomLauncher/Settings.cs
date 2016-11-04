using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace eveCustomLauncher
{   
    static class Settings
    {
        const string ECL_KEY = @"Software\EVE Custom Launcher";
        const string KEY_EXEFILE_PATH = "EvePath";
        static Log log = Log.Instance;
        static bool initCompleted = false;
        static RegistryKey eclReg;
        public static void Init()
        {
            log.WriteLine("Settings init");
            eclReg = Registry.CurrentUser.OpenSubKey(ECL_KEY);
            if (eclReg == null)
            {
                eclReg = Registry.CurrentUser.CreateSubKey(ECL_KEY);
            }

            initCompleted = true;

            if (eclReg.GetValue(KEY_EXEFILE_PATH, null) == null)
            {
                SetEveInstallationDirectory();                
            }
        }

        public static void SetEveInstallationDirectory()
        {
            if (initCompleted)
            {
                string exefilePath = EveLauncher.FindExefile();
                if (File.Exists(exefilePath))
                    eclReg.SetValue(KEY_EXEFILE_PATH, exefilePath);
                else throw new Exception("Eve client not found.");
            }
        }

        public static string GetEveInstallationDirectory()
        {
            if (initCompleted)
            {
                object exefilePath = eclReg.GetValue(KEY_EXEFILE_PATH, null);
                if (exefilePath == null)
                {
                    SetEveInstallationDirectory();
                    return GetEveInstallationDirectory();
                }
                return (string)exefilePath;
            }
            return string.Empty;
        }

        public static bool CheckIfECLPOpenHandlerExists()
        {
            string Extension = ".eclp";
            string KeyName = "ECLProfile";
            RegistryKey hkcuClasses = Registry.CurrentUser.OpenSubKey(@"Software\Classes");
            if (hkcuClasses.OpenSubKey(Extension) == null || hkcuClasses.OpenSubKey(KeyName) == null)
                return false;
            return true;
        }

        public static void SetECLPFileOpenHandler()
        {
            RegistryKey BaseKey;
            RegistryKey OpenMethod;
            RegistryKey Shell;
            RegistryKey CurrentUser;
            RegistryKey hkcuClasses = Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);
            string Extension = ".eclp";
            string KeyName = "ECLProfile";
            string OpenWith = System.Reflection.Assembly.GetEntryAssembly().Location;
            string FileDescription = "EVE Custom Launcher profile";

            BaseKey = hkcuClasses.CreateSubKey(Extension);
            BaseKey.SetValue("", KeyName);

            OpenMethod = hkcuClasses.CreateSubKey(KeyName);
            OpenMethod.SetValue("", FileDescription);
            OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "" + OpenWith + ",0");
            Shell = OpenMethod.CreateSubKey("Shell");
            Shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"/profile:%1\"");
            BaseKey.Close();
            OpenMethod.Close();
            Shell.Close();

            CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, true);
            CurrentUser.DeleteSubKey("UserChoice", false);
            CurrentUser.Close();

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }


        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
