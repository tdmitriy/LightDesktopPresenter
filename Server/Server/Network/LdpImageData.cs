using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network
{
    [ProtoContract]
    class LdpImageData
    {
        [ProtoMember(1)]
        public byte[] Compressed { get; set; }
        [ProtoMember(2)]
        public int BaseLenght { get; set; }
        [ProtoMember(3)]
        public int BlockPosition { get; set; }
    }
}
