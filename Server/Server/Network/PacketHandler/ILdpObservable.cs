using Server.Network.PacketSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    interface ILdpObservable
    {
        void AddListener(ILdpPacketHandler listener);
        void RemoveListener(ILdpPacketHandler listener);
        void RemoveListeners();
        void NotifyToAllListeners(LdpPacket packet, ILdpPacketSender channel);
    }
}
