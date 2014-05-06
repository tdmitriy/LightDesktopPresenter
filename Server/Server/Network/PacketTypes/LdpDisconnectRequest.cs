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
        public bool DisconnectFromServer { get; set; }
        [ProtoMember(2)]
        public bool DisconnectFromScreenThread { get; set; }
        [ProtoMember(3)]
        public bool DisconnectFromVolumeThread { get; set; }
    }
}
