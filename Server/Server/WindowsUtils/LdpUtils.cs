using System;
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

        public static readonly bool WINDOWS7 = IsWindows7();
        public static readonly bool WINDOWS8 = IsWindows8();


        #region Private members
        private static OperatingSystem os = Environment.OSVersion;
        private static Version vs = os.Version;
        #endregion

        #region OS detection
        private static bool IsWindows7()
        {
            Version win7version = new Version(6, 1, 7600, 0);
            if (os.Platform == PlatformID.Win32NT &&
                vs == win7version)
            {
                //it's win7
                return true;
            }
            return false;
        }

        private static bool IsWindows8()
        {
            Version win8version = new Version(6, 2, 9200, 0);
            if (os.Platform == PlatformID.Win32NT &&
                vs >= win8version)
            {
                //it's win8 or higher.
                return true;
            }
            return false;
        }
        #endregion
    }
}
