using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    public delegate void ClientConnectedHandler(EndPoint address);
    public delegate void ClientDisconnectedHandler(/*EndPoint address*/);
    public delegate void ServerShutdownHandler();
    interface ILdpServerEvents
    {
        event ClientConnectedHandler OnClientConnected;
        event ClientDisconnectedHandler OnClientDisconnected;
        event ServerShutdownHandler OnServerShutdown;
    }
}
