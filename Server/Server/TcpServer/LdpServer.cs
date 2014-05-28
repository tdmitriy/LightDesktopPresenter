using Server.Network;
using Server.TcpServer;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class LdpServer : LdpServerBase
    {
        #region Singleton impl
        private LdpServer() { }

        public static LdpServer GetInstance()
        {
            return LdpNestedServer.singleInstance;
        }

        class LdpNestedServer
        {
            internal static readonly LdpServer singleInstance = new LdpServer();
            static LdpNestedServer() { }
        }
        #endregion
    }
}
