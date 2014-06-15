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

        public LdpPreparableDesktopInfoResponse SetRemoteDesktopInfo(int width, int height)
        {
            var response = LdpPreparableDesktopInfoResponse.CreateBuilder();
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

        public LdpPreparableVolumeInfoResponse SetPreparableVolumeInfoRequest(int volume, bool mute)
        {
            var response = LdpPreparableVolumeInfoResponse.CreateBuilder();
            response.Volume = volume;
            response.IsMute = mute;
            return response.Build();
        }

        public LdpVolumeInfoResponse SetVolumeInfoResponse(int volume)
        {
            var response = LdpVolumeInfoResponse.CreateBuilder();
            response.Type = VolumeInfoType.VOLUME;
            response.Volume = volume;
            return response.Build();
        }

        public LdpVolumeInfoResponse SetVolumeInfoResponse(bool mute)
        {
            var response = LdpVolumeInfoResponse.CreateBuilder();
            response.Type = VolumeInfoType.MUTE;
            response.IsMute = mute;
            return response.Build();
        }

        public LdpPacket BuildPacket(LdpAuthResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.AUTH_RESPONSE;
            packet.AuthResponse = response;
            return packet.Build();
        }

        public LdpPacket BuildPacket(LdpPreparableDesktopInfoResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.PREPARABLE_DESKTOP_INFO_RESPONSE;
            packet.PreparableDesktopResponse = response;
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

        public LdpPacket BuildPacket(LdpPreparableVolumeInfoResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.PREPARABLE_VOLUME_INFO_RESPONSE;
            packet.PreparableVolumeInfoResponse = response;
            return packet.Build();
        }

        public LdpPacket BuildPacket(LdpVolumeInfoResponse response)
        {
            var packet = LdpPacket.CreateBuilder();
            packet.Type = PacketType.VOLUME_INFO_RESPONSE;
            packet.VolumeInfoResponse = response;
            return packet.Build();
        }
    }
}
