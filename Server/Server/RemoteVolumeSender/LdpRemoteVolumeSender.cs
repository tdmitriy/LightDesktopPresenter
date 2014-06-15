using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteVolumeSender
{
    class LdpRemoteVolumeSender : ILdpPacketHandler
    {
        public void Handle(LdpPacket packet)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
