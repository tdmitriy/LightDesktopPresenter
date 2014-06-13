using Server.ClientInfo;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Handlers
{
    class LdpClientInfoRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpPreparableInfoRequestHandler preparableRequestHandler;
        public LdpClientInfoRequestHandler()
        {
            serverHandler = LdpServer.GetInstance();
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.CLIENT_INFO_REQUEST:
                    var clientInfo = packet.ClientInfoRequest;
                    LdpClientInfo.IP = serverHandler.GetClientIPAddress;
                    LdpClientInfo.OS = clientInfo.OS;
                    LdpClientInfo.DEVICE_NAME = clientInfo.DeviceName;

                    preparableRequestHandler = new LdpPreparableInfoRequestHandler();
                    serverHandler.GetListenerChannel.AddListener(preparableRequestHandler);

                    serverHandler.GetListenerChannel.RemoveListener(this);
                    break;
            }
        }

        public void Dispose()
        {
            serverHandler = null;
            GC.SuppressFinalize(this);
        }
    }
}
