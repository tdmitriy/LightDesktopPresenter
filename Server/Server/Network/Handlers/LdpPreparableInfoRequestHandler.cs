using Server.Network.Handlers.PacketHandlerBase;
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
                            SendPreparableInfoResponse(ConnectionType.REMOTE_DESKTOP_CONTROL);
                            break;
                        case ConnectionType.REMOTE_VOLUME_CONTROL:
                            break;
                    }
                    break;
            }
        }

        private void SendPreparableInfoResponse(ConnectionType type)
        {
            switch (type)
            {
                case ConnectionType.REMOTE_DESKTOP_CONTROL:
                    SendRemoteDesktopPreparableInfo();
                    break;
                case ConnectionType.REMOTE_VOLUME_CONTROL:
                    break;
            }
        }

        private void SendRemoteDesktopPreparableInfo()
        {
            LdpPreparableInfoResponse response = packetFactory
                                .SetRemoteDesktopInfo(LdpUtils.SCREEN_WIDTH,
                                LdpUtils.SCREEN_HEIGHT);
            LdpPacket responsePacket = packetFactory.BuildPacket(response);
            serverHandler.GetSenderChannel.Send(responsePacket);

            remoteDesktopSender = new LdpRemoteDesktopSender();
            serverHandler.GetListenerChannel.AddListener(remoteDesktopSender);
            serverHandler.GetListenerChannel.RemoveListener(this);
        }

        private void SendRemoteVolumePreparableInfo()
        {
            // TODO
        }

        public void Dispose()
        {
            serverHandler = null;
            packetFactory = null;
            GC.SuppressFinalize(this);
        }
    }
}
