using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    interface ILdpServer : ILdpServerHandlers
    {
        void Start(int port);
        void Start();
        void Stop();
        void Restart();
        void DisconnectClient();

        int GetServerPort { get; }
        List<string> GetLocalIpAddressList { get; }
        string GetClientIPAddress { get; }
        
    }
}
