using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    interface ILdpTcpServer : ILdpServerEvents
    {
        Socket ClientChannel { get; }
        void Start();
        void Stop();
        void Restart();
        string GetServerIPAddress { get; }
    }
}
