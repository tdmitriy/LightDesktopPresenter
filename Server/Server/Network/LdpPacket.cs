using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Network.PacketTypes;
using ProtoBuf;

namespace Server.Network
{
    [ProtoContract]
    class LdpPacket
    {
        [ProtoMember(1, IsRequired = true)]
        public PacketType Type { get; set; }
        [ProtoMember(2)]
        public LdpAuthRequest AuthRequest { get; set; }
        [ProtoMember(3)]
        public LdpAuthResponse AuthResponse { get; set; }
        [ProtoMember(4)]
        public LdpClientInfoRequest ClientInfoRequest { get; set; }
        [ProtoMember(5)]
        public LdpScreenData ScreenData { get; set; }
        [ProtoMember(6)]
        public LdpDisconnectRequest DisconnectRequest { get; set; }

    }

    enum PacketType 
    {
		AUTH_REQUEST = 1,
		AUTH_RESPONSE = 2,

        CLIENT_INFO_REQUEST = 3,

        PREPARABLE_INFO_REQUEST = 4,
        PREPARABLE_INFO_RESPONSE = 5,
	
		SCREEN_DATA = 6,
		VOLUME_DATA = 7,
		
		MOUSE_DATA = 8,
		
		KEYBOARD_DATA = 9,
		
		DISCONNECT_REQUEST = 10,
	}
}
