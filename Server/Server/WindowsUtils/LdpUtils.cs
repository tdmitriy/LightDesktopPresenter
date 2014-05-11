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

        #region Private members
        private static OperatingSystem os = Environment.OSVersion;
        private static Version vs = os.Version;
        #endregion

        #region OS detection
        public static bool IsWindows7
        {
            get
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
        }

        public static bool IsWindows8
        {
            get
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
        }
        #endregion
    }
}
