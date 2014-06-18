using CoreAudioApi;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteVolumeSender
{
    class LdpRemoteVolumeSender : ILdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpVolumeController volumeController;
        private LdpProtocolPacketFactory packetFactory;
        private LdpPacket packetRequest;
        private LdpPreparableVolumeInfoResponse preparableVolumeResponse;
        private LdpVolumeInfoResponse volumeInfoResponse;

        public LdpRemoteVolumeSender()
        {
            serverHandler = LdpServer.GetInstance();
            packetFactory = new LdpProtocolPacketFactory();
            serverHandler.GetListenerChannel.AddListener(this);

            volumeController = new LdpVolumeController();
            volumeController.OnMute += volumeController_OnMute;
            volumeController
                .GetDevice
                .AudioEndpointVolume
                .OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        private void volumeController_OnMute(bool muteState)
        {
            SendVolumeInfoPacket(volumeController.Mute);
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.VOLUME_INFO_RESPONSE:
                    ProcessPacket(packet);
                    break;
            }
        }

        private void ProcessPacket(LdpPacket packet)
        {
            volumeInfoResponse = packet.VolumeInfoResponse;
            switch (volumeInfoResponse.Type)
            {
                case VolumeInfoType.VOLUME:
                    volumeController.VolumeValue = volumeInfoResponse.Volume;
                    break;
                case VolumeInfoType.MUTE:
                    volumeController.Mute = volumeInfoResponse.IsMute;
                    break;
            }
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (volumeController.GetTempMuteState != data.Muted)
                volumeController.Mute = data.Muted;

            volumeController.GetTempMuteState = data.Muted;
            SendVolumeInfoPacket(volumeController.VolumeValue);
        }

        private void SendVolumeInfoPacket(int volume)
        {
            volumeInfoResponse = null;
            packetRequest = null;
            volumeInfoResponse = packetFactory.SetVolumeInfoResponse(volume);
            packetRequest = packetFactory.BuildPacket(volumeInfoResponse);
            serverHandler.GetSenderChannel.Send(packetRequest);
        }

        private void SendVolumeInfoPacket(bool mute)
        {
            volumeInfoResponse = null;
            packetRequest = null;
            volumeInfoResponse = packetFactory.SetVolumeInfoResponse(mute);
            packetRequest = packetFactory.BuildPacket(volumeInfoResponse);
            serverHandler.GetSenderChannel.Send(packetRequest);
        }

        public void SendPreparableVolumeInfoPacket()
        {
            preparableVolumeResponse = null;
            packetRequest = null;

            preparableVolumeResponse = packetFactory
                .SetPreparableVolumeInfoRequest(volumeController.VolumeValue, volumeController.Mute);
            packetRequest = packetFactory.BuildPacket(preparableVolumeResponse);
            serverHandler.GetSenderChannel.Send(packetRequest);
        }

        public void Dispose()
        {
            serverHandler = null;
            volumeController = null;
            packetFactory = null;
            packetRequest = null;
            volumeInfoResponse = null;
            preparableVolumeResponse = null;
        }
    }
}
