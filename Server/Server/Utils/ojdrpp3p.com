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
            return false;
        }

        public static bool IsWindows8()
        {
            return false;
        }

        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            string Query = "Select Name from Win32_OperatingSystem";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);
            foreach (ManagementObject Win32 in searcher.Get())
            {
                string OSVersion = Win32["Name"] as string;
            }
            return result;
        }
    }

    enum VersionType
    {
        WINDOWS_7,
        WINDOWS_8
    }
}
