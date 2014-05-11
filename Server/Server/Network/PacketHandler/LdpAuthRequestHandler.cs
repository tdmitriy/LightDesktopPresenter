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
        public void Handle(LdpPacket packet)
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
