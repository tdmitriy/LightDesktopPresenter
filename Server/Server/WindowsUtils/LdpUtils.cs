﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.WindowsUtils
{
    class LdpUtils
    {
        public static readonly int SCREEN_WIDTH = 
            (int)SystemParameters.PrimaryScreenWidth;
        public static readonly int SCREEN_HEIGHT = 
            (int)SystemParameters.PrimaryScreenHeight;

        #region Private members
        private static OperatingSystem os = Environment.OSVersion;
        private static Version vs = os.Version;
        #endregion

        #region OS detection
        public static bool IsWindows7
        {
            get
            {
                if (os.Platform == PlatformID.Win32NT)
                {
                    if (vs.Major == 6 && vs.Minor == 1)
                        //it's win7
                        return true;
                }

                return false;
            }
        }

        public static bool IsWindows8
        {
            get
            {
                if (os.Platform == PlatformID.Win32NT)
                {
                    if (vs.Major == 6 && vs.Minor >= 2)
                        //it's win8 or higher
                        return true;
                }
                return false;
            }
        }

        public static bool CheckStartupWindowsVersion(ServerWindow window)
        {
            if (LdpUtils.IsWindows7)
            {
                LdpLog.Info("Current OS: Windows 7.");
                return true;
            }
            else if (LdpUtils.IsWindows8)
            {
                LdpLog.Info("Current OS: Windows 8.");
                return true;
            }
            else
            {
                MessageBox.Show("Unsuported windows version.\nWorks only on Windows 7 or higher..",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                window.Close();
                return false;
            }
        }
        #endregion
    }
}
