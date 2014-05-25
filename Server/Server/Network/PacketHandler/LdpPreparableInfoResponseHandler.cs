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
        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.PREPARABLE_INFO_REQUEST:
                    var request = packet.PreparableInfoRequest;
                    switch (request.Type)
                    {
                        case ConnectionType.REMOTE_DESKTOP_CONTROL:
                            // sends response
                            // start screen thread and send screen data
                            break;
                        case ConnectionType.REMOTE_VOLUME_CONTROL:
                            break;
                    }
                    break;
            }
        }
    }
}
