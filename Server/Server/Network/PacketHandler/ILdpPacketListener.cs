using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    interface ILdpPacketListener
    {
        void OnPacketReceived(LdpPacket packet);
    }
}
