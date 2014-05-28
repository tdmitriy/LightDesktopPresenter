using Server.Network.Handlers.PacketHandlerBase;
using Server.Network.PacketTypes;
using Server.Protocol;
using Server.RemoteDesktopSender;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Handlers
{
    class LdpPreparableInfoRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpProtocolPacketFactory packetFactory;
        private LdpRemoteDesktopSender remoteDesktopSender; 
        public LdpPreparableInfoRequestHandler()
        {
            packetFactory = new LdpProtocolPacketFactory();
            serverHandler = LdpServer.GetInstance();
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.PREPARABLE_INFO_REQUEST:
                    var request = packet.PreparableInfoRequest;
                    switch (request.Type)
                    {
                        case ConnectionType.REMOTE_DESKTOP_CONTROL:
                            LdpPreparableInfoResponse response = packetFactory
                                .SetRemoteDesktopInfo(LdpUtils.SCREEN_WIDTH, 
                                LdpUtils.SCREEN_HEIGHT);
                            LdpPacket responsePacket = packetFactory.BuildPacket(response);
                            serverHandler.GetSenderChannel.Send(responsePacket);

                            serverHandler.GetListenerChannel.RemoveListener(this);

                            remoteDesktopSender = new LdpRemoteDesktopSender();
                            break;
                        case ConnectionType.REMOTE_VOLUME_CONTROL:
                            break;
                    }
                    break;
            }
        }

        public void Dispose()
        {
            serverHandler = null;
            packetFactory = null;
            if (remoteDesktopSender != null)
                remoteDesktopSender.Dispose();
            remoteDesktopSender = null;
        }
    }
}
