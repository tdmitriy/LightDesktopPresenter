using Server.Properties;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.UserSettings
{
    class LdpUserSettings
    {
        private string userPassword;
        public LdpUserSettings() 
        {
            InitSettings();
        }

        private void InitSettings()
        {
            userPassword = Settings.Default.Password;
        }

        public string GetPassword
        {
            get { return userPassword; }
        }

        public void SetPassword(string password)
        {
            Settings.Default.Password = password;
            Settings.Default.Save();
            string success = "User settings: password updated successfully.";
            LdpLog.Info(success);
            
        }

    }
}
