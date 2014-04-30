using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network
{
    [ProtoContract]
    class LdpDisconnectRequest
    {
        [ProtoMember(1)]
        public bool IsDisconnect { get; set; }
    }
}
