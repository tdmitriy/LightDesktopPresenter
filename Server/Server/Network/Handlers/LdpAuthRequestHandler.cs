using Server.Network.Handlers.PacketHandlerBase;
using Server.Network.PacketTypes;
using Server.Protocol;
using Server.TcpServer;
using Server.UserSettings;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Network.Handlers
{
    class LdpAuthRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpUserSettings userSettings;
        private LdpProtocolPacketFactory packetFactory;
        private LdpClientInfoRequestHandler infoRequest;
        private LdpAuthResponse authResponse;
        private LdpPacket responsePacket;


        private string settingsPassword;
        public LdpAuthRequestHandler()
        {
            serverHandler = LdpServer.GetInstance();
            packetFactory = new LdpProtocolPacketFactory();
            userSettings = new LdpUserSettings();
            settingsPassword = userSettings.GetPassword;
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.AUTH_REQUEST:
                    //if ok - send auth response
                    var auth = packet.AuthRequest;
                    CheckPassword(auth.Password);
                    break;
            }
        }

        private void CheckPassword(string requestPassword)
        {
            if (requestPassword == settingsPassword)
            {
                infoRequest = new LdpClientInfoRequestHandler();
                serverHandler.GetListenerChannel.AddListener(infoRequest);

                authResponse = packetFactory.SetAuthResponse(true);
                responsePacket = packetFactory.BuildPacket(authResponse);
                serverHandler.GetSenderChannel.Send(responsePacket);
                LdpLog.Info("Auth successfull.");
                serverHandler.GetListenerChannel.RemoveListener(this);
            }
            else
            {
                authResponse = packetFactory.SetAuthResponse(false);
                responsePacket = packetFactory.BuildPacket(authResponse);
                serverHandler.GetSenderChannel.Send(responsePacket);
                LdpLog.Info("Auth failed: wrong password.");
            }
        }

        public void Dispose()
        {
            authResponse = null;
            responsePacket = null;
            serverHandler = null;
            userSettings = null;
            packetFactory = null;
        }
    }
}
