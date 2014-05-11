using Server.Network.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    
    interface ILdpDisconnectPacketHandler
    {
        void Handle(LdpDisconnectRequest packet);
    }
}
