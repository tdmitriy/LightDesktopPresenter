using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    class OSDetector
    {
        private static OperatingSystem os = Environment.OSVersion;
        private static Version vs = os.Version;

        public static bool IsWindows7()
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

        public static bool IsWindows8()
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
}
