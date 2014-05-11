using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpAuthResponse
    {
        [ProtoMember(1, IsRequired = true)]
        public bool isSuccess { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public ResponseMessageType Type { get; set; }

        
    }
    enum ResponseMessageType
    {
        WRONG_PASSWORD = 1,
        EMPTY_PASSWORD = 2,
    }
}
