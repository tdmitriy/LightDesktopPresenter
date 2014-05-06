using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    interface ILdpTcpServer : ILdpServerEvents
    {
        void Start();
        void Stop();
        void Restart();
        string GetServerIPAddress { get; }
    }
}
