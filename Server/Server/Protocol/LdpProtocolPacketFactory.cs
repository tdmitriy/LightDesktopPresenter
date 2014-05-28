using Server.Network;
using Server.Network.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    class LdpProtocolPacketFactory
    {
        public LdpAuthResponse SetAuthResponse(bool authResult)
        {
            var response = new LdpAuthResponse();
            response.isSuccess = authResult;
            return response;
        }

        public LdpPreparableInfoResponse SetRemoteDesktopInfo(int width, int height)
        {
            var response = new LdpPreparableInfoResponse();
            response.ScreenWidth = width;
            response.ScreenHeight = height;
            return response;
        }

        public LdpDisconnectRequest SetDisconnectRequest(DisconnectionType type)
        {
            var response = new LdpDisconnectRequest();
            response.Type = type;
            return response;
        }

        public LdpScreenResponse SetScreenResponse(byte[] compressed, 
            int baseLenght, LdpRectangle rect)
        {
            var response = new LdpScreenResponse();
            response.CompressedScreen = compressed;
            response.BaseLenght = baseLenght;
            response.Rect = rect;
            return response;
        }

        public LdpPacket BuildPacket(LdpAuthResponse response)
        {
            var packet = new LdpPacket();
            packet.Type = PacketType.AUTH_RESPONSE;
            packet.AuthResponse = response;
            return packet;
        }

        public LdpPacket BuildPacket(LdpPreparableInfoResponse response)
        {
            var packet = new LdpPacket();
            packet.Type = PacketType.PREPARABLE_INFO_RESPONSE;
            packet.PreparableInfoResponse = response;
            return packet;
        }

        public LdpPacket BuildPacket(LdpDisconnectRequest response)
        {
            var packet = new LdpPacket();
            packet.Type = PacketType.DISCONNECT_REQUEST;
            packet.DisconnectRequest = response;
            return packet;
        }

        public LdpPacket BuildPacket(LdpScreenResponse response)
        {
            var packet = new LdpPacket();
            packet.Type = PacketType.SCREEN_RESPONSE;
            packet.ScreenResponse = response;
            return packet;
        }
    }
}
