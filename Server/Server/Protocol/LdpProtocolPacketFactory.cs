using Google.ProtocolBuffers;
using Server.Network;
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
            var response = LdpAuthResponse.CreateBuilder();
            response.IsSuccess = authResult;
            return response.Build();
        }

        public LdpPreparableInfoResponse SetRemoteDesktopInfo(int width, int height)
        {
            var response = LdpPreparableInfoResponse.CreateBuilder();
            response.ScreenWidth = width;
            response.ScreenHeight = height;
            return response.Build();
        }

        public LdpDisconnectRequest SetDisconnectRequest(DisconnectionType type)
        {
            var response = LdpDisconnectRequest.CreateBuilder();
            response.Type = type;
            return response.Build();
        }

        public LdpScreenResponse SetScreenResponse(byte[] compressed, 
            int baseLenght, LdpRectangle.Builder rect)
        {
            var response = LdpScreenResponse.CreateBuilder();
            ByteString bytes = ByteString.CopyFrom(compressed);
            response.CompressedScreen = bytes;
            response.BaseLenght = baseLenght;
            response.SetRect(rect);
            return response.Build();
        }

        public LdpPacket BuildPacket(LdpAuthResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.AUTH_RESPONSE;
            packet.AuthResponse = response;
            return packet.Build();
        }

        public LdpPacket BuildPacket(LdpPreparableInfoResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.PREPARABLE_INFO_RESPONSE;
            packet.PreparableInfoResponse = response;
            return packet.Build();
        }

        public LdpPacket BuildPacket(LdpDisconnectRequest response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.DISCONNECT_REQUEST;
            packet.DisconnectRequest = response;
            return packet.Build();
        }

        public LdpPacket BuildPacket(LdpScreenResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.SCREEN_RESPONSE;
            packet.ScreenResponse = response;
            return packet.Build();
        }
    }
}
