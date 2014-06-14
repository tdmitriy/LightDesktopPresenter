using Server.ClientInfo;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.WindowsUtils;
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
        private StringBuilder lblText;
        public LdpClientInfoRequestHandler()
        {
            lblText = new StringBuilder();
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
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
                    lblText.AppendLine("Client connected:");
                    lblText.AppendLine("IP: " + LdpClientInfo.IP);
                    lblText.AppendLine("Device: " + LdpClientInfo.DEVICE_NAME);
                    lblText.AppendLine("OS: " + LdpClientInfo.OS);

                    LdpLabelStatus.GetInstance().StateText = lblText.ToString();

                    preparableRequestHandler = new LdpPreparableInfoRequestHandler();
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
