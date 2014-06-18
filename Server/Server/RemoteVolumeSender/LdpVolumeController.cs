using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteVolumeSender
{
    class LdpVolumeController : ILdpVolumeController
    {

        private MMDevice device;
        private MMDeviceEnumerator devEnum;

        private bool tempMuteState = false;

        private const float MULT_VOLUME_COEF = 100.0f;
        private const float DIV_VOLUME_COEF = 0.01f;

        public delegate void OnMuteEventHandler(bool muteState);
        public event OnMuteEventHandler OnMute;

        public LdpVolumeController()
        {
            devEnum = new MMDeviceEnumerator();
            device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            GetTempMuteState = device.AudioEndpointVolume.Mute;
        }

        public MMDevice GetDevice
        {
            get { return device; }
        }

        public bool GetTempMuteState
        {
            get { return tempMuteState; }
            set { tempMuteState = value; }
        }

        public int VolumeValue
        {
            get
            {
                return (int)Math.Round(device.AudioEndpointVolume
                    .MasterVolumeLevelScalar * MULT_VOLUME_COEF);
            }
            set
            {
                if (value >= 0 && value <= 100)
                    device.AudioEndpointVolume
                        .MasterVolumeLevelScalar = value * DIV_VOLUME_COEF;
            }
        }

        public bool Mute
        {
            get { return device.AudioEndpointVolume.Mute; }
            set 
            { 
                device.AudioEndpointVolume.Mute = value;
                if (OnMute != null)
                    OnMute(value);
            }
        }
    }
}
