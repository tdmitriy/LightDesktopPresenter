using Server.TcpServer.PacketListener;
using Server.TcpServer.PacketSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    interface ILdpServerHandlers
    {
        LdpPacketListener GetListenerChannel { get; }
        LdpPacketSender GetSenderChannel { get; }
        Socket GetSocketChannel { get; }
    }
}
