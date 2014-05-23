using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network.PacketTypes
{
    [ProtoContract]
    class LdpPreparableInfoResponse
    {
        [ProtoMember(1)]
        public int ScreenWidth { get; set; }

        [ProtoMember(2)]
        public int ScreenHeight { get; set; }

    }
}
