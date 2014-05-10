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
        private List<ILdpPacketHandler> listeners;
        public void AddListener(ILdpPacketHandler listener)
        {
            if (listeners == null)
                listeners = new List<ILdpPacketHandler>();
            listeners.Add(listener);
        }

        public void RemoveListeners()
        {
            if (listeners != null)
            {
                listeners.Clear();
                listeners = null;
            }
        }

        public virtual void OnPacketReceived(LdpPacket packet)
        {
            if (listeners != null && listeners.Count != 0)
            {
                foreach (var listener in listeners)
                {
                    ILdpPacketHandler ipl = listener;
                    ipl.Handle(packet);
                }
            }
        }
        #endregion

        protected abstract void Handle();
    }
}
