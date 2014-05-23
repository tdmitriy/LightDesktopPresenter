using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpAuthRequest
    {
        [ProtoMember(1)]
        public string Password { get; set; }
    }
}
