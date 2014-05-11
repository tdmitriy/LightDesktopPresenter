using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpScreenData
    {
        [ProtoMember(1)]
        public byte[] CompressedScreen { get; set; }
        [ProtoMember(2)]
        public int BaseLenght { get; set; }
        [ProtoMember(3)]
        public int BlockPosition { get; set; }
        [ProtoMember(4)]
        public LdpRectangle Rect { get; set; }
    }

    [ProtoContract]
    internal class LdpRectangle
    {
        [ProtoMember(1)]
        public int Left { get; set; }
        [ProtoMember(2)]
        public int Top { get; set; }
        [ProtoMember(3)]
        public int Right { get; set; }
        [ProtoMember(4)]
        public int Bottom { get; set; }
    }
}
