using Server.Network.PacketSender;
using Server.Network.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.PacketHandler
{
    abstract class LdpPacketHandler : ILdpObservable, IDisposable
    {
        private static object MUTEX = new object();
        protected abstract void Handle();

        private List<ILdpPacketHandler> listeners;
        public LdpPacketHandler()
        {
            listeners = new List<ILdpPacketHandler>();
        }   

        #region Observer pattern impl
        public void AddListener(ILdpPacketHandler listener)
        {
            lock (MUTEX)
            {
                if (!listeners.Contains(listener))
                    listeners.Add(listener);
            }
        }

        public void RemoveListener(ILdpPacketHandler listener)
        {
            lock (MUTEX)
            {
                if (listeners.Contains(listener))
                    listeners.Remove(listener);
            }
        }

        public void RemoveListeners()
        {
            lock (MUTEX)
            {
                if (listeners != null)
                {
                    listeners.Clear();
                    listeners = null;
                }
            }
        }

        public void NotifyToAllListeners(LdpPacket packet, ILdpPacketSender channel)
        {
            if (listeners != null && listeners.Count != 0)
            {
                foreach (var listener in listeners)
                {
                    ILdpPacketHandler ipl = (ILdpPacketHandler)listener;
                    ipl.Handle(packet, channel);
                }
            }
        }
        #endregion

        public void Dispose()
        {
            RemoveListeners();
        }
    }
}
