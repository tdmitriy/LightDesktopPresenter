using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpDisconnectRequest
    {
        [ProtoMember(1, IsRequired = true)]
        public DisconnectionType Type { get; set; }
    }

    enum DisconnectionType
    {
        FROM_SERVER = 1,
        FROM_SCREEN_THREAD = 2,
        FROM_VOLUME_THREAD = 3
    }
}
