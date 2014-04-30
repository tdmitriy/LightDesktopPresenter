using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    class Log
    {
        private static string DEBUG = "DEBUG";
        private static string INFO = "INFO";
        private static string WARNING = "WARNING";
        private static string ERROR = "ERROR";
        private static string FATAL = "FATAL";
        public static void Debug(string message)
        {
            Trace.WriteLine(String.Format("{0}: {1}.", DEBUG, message));
        }

        public static void Info(string message)
        {
            Trace.WriteLine(String.Format("{0}: {1}.", INFO, message));
        }

        public static void Warning(string message)
        {
            Trace.WriteLine(String.Format("{0}: {1}.", WARNING, message));
        }

        public static void Error(string message)
        {
            Trace.WriteLine(String.Format("{0}: {1}.", ERROR, message));
        }

        public static void Fatal(string message)
        {
            Trace.WriteLine(String.Format("{0}: {1}.", FATAL, message));
        }

    }
}
