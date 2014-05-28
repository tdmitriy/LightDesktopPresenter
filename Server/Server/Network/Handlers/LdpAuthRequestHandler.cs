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
        private string password;
        public LdpAuthRequestHandler()
        {
            serverHandler = LdpServer.GetInstance();
            packetFactory = new LdpProtocolPacketFactory();
            userSettings = new LdpUserSettings();
            password = userSettings.GetPassword;
            /*if (password == "")
                MessageBox.Show("Password is empty.");
            else
                MessageBox.Show("Passworid is not empty=" + password);*/
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.AUTH_REQUEST:
                    //if ok - send auth response
                    var auth = packet.AuthRequest;
                    LdpAuthResponse authResponse;
                    LdpPacket responsePacket;

                    if (auth.Password == password)
                    {
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
                    
                    break;
            }
        }

        public void Dispose()
        {
            serverHandler = null;
            userSettings = null;
            packetFactory = null;
        }
    }
}
