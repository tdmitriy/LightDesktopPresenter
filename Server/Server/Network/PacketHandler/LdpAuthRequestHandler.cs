using Server.Network.PacketSender;
using Server.Network.PacketTypes;
using Server.TcpServer;
using Server.UserSettings;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    class LdpAuthRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler = LdpServer.GetInstance();
        private LdpUserSettings userSettings;
        private string password;
        public LdpAuthRequestHandler()
        {
            userSettings = new LdpUserSettings();
            password = userSettings.GetPassword;
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.AUTH_REQUEST:
                    //if ok - send auth response
                    var auth = packet.AuthRequest;
                    LdpAuthResponse authResponse = new LdpAuthResponse();
                    LdpPacket responsePacket = new LdpPacket();
                    responsePacket.Type = PacketType.AUTH_RESPONSE;

                    if (auth.Password == password)
                    {
                        authResponse.isSuccess = true;
                        responsePacket.AuthResponse = authResponse;
                        serverHandler.GetSenderChannel.Send(responsePacket);
                        LdpLog.Info("Auth successfull.");
                    }
                    else
                    {
                        authResponse.isSuccess = false;
                        responsePacket.AuthResponse = authResponse;
                        serverHandler.GetSenderChannel.Send(responsePacket);
                        LdpLog.Info("Auth failed: wrong password.");
                    }
                    break;
            }
        }
    }
}
