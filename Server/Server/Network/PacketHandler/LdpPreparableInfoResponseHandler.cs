using Server.Network.PacketSender;
using Server.Network.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    class LdpPreparableInfoResponseHandler : ILdpPacketHandler
    {
        public void Handle(LdpPacket packet, ILdpPacketSender channel)
        {
            switch (packet.Type)
            {
                case PacketType.PREPARABLE_INFO_REQUEST:
                    //send response
                    break;
            }
        }
    }
}
