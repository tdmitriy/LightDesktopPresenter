using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpScreenResponse
    {
        [ProtoMember(1, IsRequired = true)]
        public byte[] CompressedScreen { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int BaseLenght { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public LdpRectangle Rect { get; set; }
    }

    [ProtoContract]
    internal class LdpRectangle
    {
        [ProtoMember(1, IsRequired = true)]
        public int Left { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int Top { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public int Right { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public int Bottom { get; set; }
    }
}
