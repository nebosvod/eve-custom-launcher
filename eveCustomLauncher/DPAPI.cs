using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security;

namespace eveCustomLauncher
{
    public class DPAPI
    {
        private string _userName;
        private byte[] _password;
        private string _settingsProfile;
        private bool addExt = false;

        public DPAPI(string fileName)
        {
            if (!File.Exists(fileName))
                if (!File.Exists(fileName + ".eclp"))
                    throw new Exception(string.Format("Profile file {0} not found!", fileName));
                else addExt = true;

            int pos = 0;
            byte[] data = File.ReadAllBytes(fileName + (addExt ? ".eclp" : string.Empty));
            int usernameLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _userName = Encoding.ASCII.GetString(data, pos, usernameLength);
            pos += usernameLength;
            int settingsProfileLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _settingsProfile = Encoding.ASCII.GetString(data, pos, settingsProfileLength);
            pos += settingsProfileLength;
            int protectedPasswordLength = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            _password = new byte[protectedPasswordLength];
            Array.Copy(data, pos, _password, 0, protectedPasswordLength);
        }

        public string GetUserName()
        {
            return _userName;
        }

        public string GetSettingsProfile()
        {
            return _settingsProfile;
        }

        public string GetPassword()
        {
            byte[] entropy = Encoding.ASCII.GetBytes(Environment.MachineName + _userName);
            return Encoding.ASCII.GetString(ProtectedData.Unprotect(_password, entropy, DataProtectionScope.CurrentUser));
        }

        public static void CreateKeyFile(string fileName, string userName, string password, string settingsProfile)
        {
            if(File.Exists(fileName))
                throw new Exception(string.Format("File {0} already exists!", fileName));
            byte[] entropy = Encoding.ASCII.GetBytes(Environment.MachineName + userName);
            byte[] data = Encoding.ASCII.GetBytes(password);
            byte[] protectedData = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
            byte[] userNameData = Encoding.ASCII.GetBytes(userName);
            byte[] settingsProfileData = Encoding.ASCII.GetBytes(settingsProfile);
            using (FileStream stream = File.Create(fileName))
            {
                stream.Write(BitConverter.GetBytes(userNameData.Length), 0, sizeof(int));
                stream.Write(userNameData, 0, userNameData.Length);

                stream.Write(BitConverter.GetBytes(settingsProfileData.Length), 0, sizeof(int));
                stream.Write(settingsProfileData, 0, settingsProfileData.Length);

                stream.Write(BitConverter.GetBytes(protectedData.Length), 0, sizeof(int));
                stream.Write(protectedData, 0, protectedData.Length);
            }
        }
    }
}
