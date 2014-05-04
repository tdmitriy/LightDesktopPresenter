using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Server.Network
{
    [ProtoContract]
    class LdpPacket
    {
        [ProtoMember(1)]
        public PacketType Type { get; set; }
        [ProtoMember(2)]
        public LdpAuthRequest AuthRequest { get; set; }
        [ProtoMember(3)]
        public LdpAuthResponse AuthResponse { get; set; }
        [ProtoMember(4)]
        public LdpScreenData ScreenData { get; set; }
        [ProtoMember(5)]
        public LdpDisconnectRequest DisconnectRequest { get; set; }

    }

    enum PacketType 
    {
		AUTH_REQUEST 			= 1,
		AUTH_RESPONSE 			= 2,
	
		IMG_DATA 				= 3,
		VOLUME_DATA 			= 4,
		
		MOUSE_POS_DATA 			= 5,
		CURSOR_DATA				= 6,
		
		KEYBOARD_DATA			= 7,
		
		DISCONNECT_REQUEST 		= 8,
	}
}
