using Server.Network.PacketSender;
using Server.Network.PacketTypes;
using Server.WindowsUtils;
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
                {
                    listeners.Remove(listener);
                    LdpLog.Info(String.Format("Listener removed: {0}.", listener.GetType()));
                }
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

        public void NotifyToAllListeners(LdpPacket packet)
        {
            if (listeners != null && listeners.Count != 0)
            {
                foreach (var listener in listeners.ToList())
                {
                    ILdpPacketHandler ipl = (ILdpPacketHandler)listener;
                    ipl.Handle(packet);
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
