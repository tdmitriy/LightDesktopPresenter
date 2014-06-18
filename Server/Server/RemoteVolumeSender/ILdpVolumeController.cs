using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteVolumeSender
{
    interface ILdpVolumeController
    {
        int VolumeValue { get; set; }
        bool Mute { get; set; }
    }
}
