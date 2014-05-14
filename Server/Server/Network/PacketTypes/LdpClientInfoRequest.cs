using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpClientInfoRequest
    {
        [ProtoMember(1, IsRequired = true)]
        public string IP { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string OS { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string DeviceName { get; set; }
    }
}
