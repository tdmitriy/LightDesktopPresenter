using ProtoBuf;
using Server.LdpThreads;
using Server.Network;
using Server.Network.PacketHandler;
using Server.Network.PacketSender;
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

namespace Server.Network
{
    class LdpPacketListener : LdpPacketHandler
    {
        private ILdpTcpServer serverHandler;
        private ILdpPacketSender channel;
        private LdpPacketListenerThread listenerThread;
        public LdpPacketListener(ILdpTcpServer serverHandler, ILdpPacketSender channel)
        {
            this.serverHandler = serverHandler;
            this.channel = channel;
            listenerThread = new LdpPacketListenerThread();
            listenerThread.Start(new Action(Handle));
        }

        protected override void Handle()
        {
            try
            {
                using (var stream = new NetworkStream(serverHandler.ClientChannel))
                {
                    var packet = Serializer.DeserializeWithLengthPrefix<LdpPacket>(stream, PrefixStyle.Base128);
                    //base - notify to all listeners
                    NotifyToAllListeners(packet, channel);
                }
            }
            catch (IOException ioexc)
            {
                LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + ioexc.Message);
                Restart();
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + sockexc.Message);
                Restart();
            }
            catch (Exception ex)
            {
                LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + ex.Message);
                Restart();
            }
        }
        private void Restart()
        {
            listenerThread.Stop();
            serverHandler.Restart();
        }
    }
}
