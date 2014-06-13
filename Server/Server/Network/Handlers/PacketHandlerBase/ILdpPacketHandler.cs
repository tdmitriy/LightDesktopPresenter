using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Handlers.PacketHandlerBase
{
    interface ILdpPacketHandler : IDisposable
    {
        void Handle(LdpPacket packet);
    }
}
