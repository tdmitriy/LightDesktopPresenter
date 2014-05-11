using ProtoBuf;
using Server.Network;
using Server.Network.PacketHandler;
using Server.Network.PacketSender;
using Server.TcpServer;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
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
        private Thread listenerThread;
        private bool workingThread = false;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        public LdpPacketListener(ILdpTcpServer serverHandler, ILdpPacketSender channel) 
        {
            this.serverHandler = serverHandler;
            this.channel = channel;
            StartListenerThread();
        }

        private void StartListenerThread()
        {
            workingThread = true;
            LdpLog.Info("Server listener started.");
            listenerThread = new Thread(new ThreadStart(Run));
            listenerThread.Start();
        }

        /*private void Restart()
        {
            LdpLog.Info("Restarting server listener thread.");
            Stop();
            StartListenerThread();
            LdpLog.Info("Restarting server listener thread completed.");
        }*/

        private void Run()
        {
            while (workingThread)
            {
                Handle();
            }
        }

        private void Stop()
        {
            try
            {
                workingThread = false;
                LdpLog.Info("Server pakcet listener thread interrupted.");
            }
            catch (Exception ex)
            {
                LdpLog.Error("LdpPacketListener error: Stop() method throw:\n" + ex.Message);
            }
        }

        protected override void Handle()
        {
            try
            {
            using (var stream = new NetworkStream(serverHandler.ClientChannel))
                {
                    var packet = Serializer.DeserializeWithLengthPrefix<LdpPacket>(stream, PrefixStyle.Base128);
                    //base
                    Handle(packet, channel);
                }
            }
            catch (SocketException sockExc)
            {
                if (sockExc.ErrorCode == CLIENT_DISCON_ERROR_CODE)
                {
                }
                else
                {
                    LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + sockExc.Message);
                }
                serverHandler.Restart();
            }
            catch (Exception ex)
            {
                LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + ex.Message);
                serverHandler.Restart();
            }
        }
    }
}
