using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.RemoteDesktopSender;
using Server.RemoteVolumeSender;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Network.Handlers
{
    class LdpPreparableInfoRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpProtocolPacketFactory packetFactory;
        public LdpPreparableInfoRequestHandler()
        {
            packetFactory = new LdpProtocolPacketFactory();
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
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
                            SendRemoteDesktopPreparableInfo();
                            break;
                        case ConnectionType.REMOTE_VOLUME_CONTROL:
                            SendRemoteVolumePreparableInfo();
                            break;
                    }
                    break;
            }
        }

        private void SendRemoteDesktopPreparableInfo()
        {
            var response = packetFactory
                                .SetRemoteDesktopInfo(LdpUtils.SCREEN_WIDTH,
                                LdpUtils.SCREEN_HEIGHT);
            var responsePacket = packetFactory.BuildPacket(response);
            serverHandler.GetSenderChannel.Send(responsePacket);
            var remoteDesktopSender = new LdpRemoteDesktopSender();
            
            serverHandler.GetListenerChannel.RemoveListener(this);
        }

        private void SendRemoteVolumePreparableInfo()
        {
            var remoteVolumeSender = new LdpRemoteVolumeSender();
            remoteVolumeSender.SendPreparableVolumeInfoPacket();
            serverHandler.GetListenerChannel.RemoveListener(this);
        }

        public void Dispose()
        {
            serverHandler = null;
            packetFactory = null;
            GC.SuppressFinalize(this);
        }
    }
}
