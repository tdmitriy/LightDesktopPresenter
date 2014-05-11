using Server.Network.PacketSender;
using Server.Network.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    class LdpAuthRequestHandler : ILdpPacketHandler
    {
        public void Handle(LdpPacket packet, ILdpPacketSender channel)
        {
            switch (packet.Type)
            {
                case PacketType.AUTH_REQUEST:
                    //if ok - send auth response
                    break;
            }
        }
    }
}
