using Server.Network;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpServer.PacketSender
{
    interface ILdpPacketSender
    {
        void Send(LdpPacket packet);
    }
}
