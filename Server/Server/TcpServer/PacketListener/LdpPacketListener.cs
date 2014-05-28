using ProtoBuf;
using Server.LdpThreads;
using Server.Network;
using Server.Network.Handlers.PacketHandlerBase;
using Server.TcpServer;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.TcpServer.PacketListener
{
    class LdpPacketListener : LdpPacketHandler
    {
        private LdpServer serverHandler;
        private LdpPacketListenerThread listenerThread;
        public LdpPacketListener()
        {
            listenerThread = new LdpPacketListenerThread();
            serverHandler = LdpServer.GetInstance();
            listenerThread.Start(new Action(Handle));
        }

        protected override void Handle()
        {
            try
            {
                using (var stream = new NetworkStream(serverHandler.GetSocketChannel))
                {
                    var packet = Serializer.DeserializeWithLengthPrefix<LdpPacket>(stream, 
                        PrefixStyle.Base128);
                    //base - notify to all listeners
                    NotifyToAllListeners(packet);
                }
            }
            catch (IOException ioexc)
            {
                LdpLog.Error("LdpPacketListener IOException: Handle() method throw:\n" + ioexc.Message);
                Restart();
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("LdpPacketListener SocketException: Handle() method throw:\n" + sockexc.Message);
                Restart();
            }
            catch (ArgumentNullException ex)
            {
                LdpLog.Error("LdpPacketListener Exception: Handle() method throw:\n" + ex.Message);
                Restart();
            }
        }
        private void Restart()
        {
            listenerThread.Stop();
            listenerThread.Dispose();
            serverHandler.Restart();
        }

        public override void Dispose()
        {
            base.Dispose();
            serverHandler = null;
            if (listenerThread != null)
            {
                listenerThread.Stop();
                listenerThread.Dispose();
            }
            listenerThread = null;
        }

    }
}
