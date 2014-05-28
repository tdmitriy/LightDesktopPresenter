using Server.Network.PacketTypes;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Handlers.PacketHandlerBase
{
    abstract class LdpPacketHandler : ILdpObservable, IDisposable
    {
        protected abstract void Handle();

        private List<ILdpPacketHandler> listeners;
        public LdpPacketHandler()
        {
            listeners = new List<ILdpPacketHandler>();
        }

        #region Observer pattern impl
        public void AddListener(ILdpPacketHandler listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void RemoveListener(ILdpPacketHandler listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
                listener.Dispose();
                LdpLog.Info(String.Format("Listener removed: {0}.", listener.GetType()));
                listener = null;
            }
        }

        public void RemoveListeners()
        {
            if (listeners != null && listeners.Count != 0)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                {
                    listeners[i].Dispose();
                    listeners[i] = null;
                    listeners.Remove(listeners[i]);
                }
                listeners = null;
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

        public virtual void Dispose()
        {
            RemoveListeners();
            listeners = null;
        }


        public List<ILdpPacketHandler> GetListenersList
        {
            get 
            {
                if (listeners != null && listeners.Count != 0)
                    return listeners;
                else return new List<ILdpPacketHandler>();
            }
        }
    }
}
