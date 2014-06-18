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
    class LdpDisconnectRequestHandler : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        public LdpDisconnectRequestHandler()
        {
            serverHandler = LdpServer.GetInstance();
        }
        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.DISCONNECT_REQUEST:
                    var discon = packet.DisconnectRequest;
                    switch (discon.Type)
                    {
                        case DisconnectionType.FROM_SERVER:
                            serverHandler.GetListenerChannel.RemoveListener(this);
                            serverHandler.Restart();
                            break;
                    }
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
