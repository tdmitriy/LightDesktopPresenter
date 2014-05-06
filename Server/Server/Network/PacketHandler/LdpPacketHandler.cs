using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    abstract class LdpPacketHandler : ILdpPacketHandler
    {
        #region Observer pattern impl
        private List<ILdpPacketListener> listeners;
        public void AddListener(ILdpPacketListener listener)
        {
            if (listeners == null)
                listeners = new List<ILdpPacketListener>();
            listeners.Add(listener);
        }

        public virtual void OnPacketReceived(LdpPacket packet)
        {
            if (listeners != null && listeners.Count != 0)
            {
                foreach (var listener in listeners)
                {
                    ILdpPacketListener ipl = listener;
                    ipl.OnPacketReceived(packet);
                }
            }
        }
        #endregion

        public abstract void Handle();
    }
}
