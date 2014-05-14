using Server.ClientInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    class LdpClientInfoRequestHandler : ILdpPacketHandler
    {
        public void Handle(LdpPacket packet, PacketSender.ILdpPacketSender channel)
        {
            switch (packet.Type)
            {
                case PacketType.CLIENT_INFO_REQUEST:
                    var clientInfo = packet.ClientInfoRequest;
                    LdpClientInfo.IP = clientInfo.IP;
                    LdpClientInfo.OS = clientInfo.OS;
                    LdpClientInfo.DEVICE_NAME = clientInfo.DeviceName;
                    break;
            }
        }
    }
}
