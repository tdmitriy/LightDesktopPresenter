using ProtoBuf;
using Server.Network;
using Server.Network.PacketHandler;
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
        private Socket socket;
        //private Thread listenerThread;
        private bool LISTENER = false;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        public LdpPacketListener(Socket socket) 
        {
            this.socket = socket;
            StartListenerThread();
        }

        private void StartListenerThread()
        {
            LISTENER = true;
            LdpLog.Info("Server listener started.");
            Run();
            //listenerThread = new Thread(new ThreadStart(Run));
            //listenerThread.Start();
        }

        private void Restart()
        {
            /*LdpLog.Info("Restarting server listener thread.");
            Stop();
            StartListenerThread();
            LdpLog.Info("Restarting server listener thread completed.");*/
        }

        private void Run()
        {
            while (LISTENER)
            {
                Handle();
            }
        }

        private void Stop()
        {
            try
            {
                LISTENER = false;
                LdpLog.Info("Server packet listener stopped.");
                /*if (listenerThread.IsAlive)
                {
                    listenerThread.Interrupt();
                    listenerThread.Abort();
                    LdpLog.Info("Server pakcet listener thread interrupted.");
                }*/
            }
            catch (Exception ex)
            {
                LdpLog.Error("LdpPacketListener error: Stop() method throw:\n" + ex.Message);
            }
        }

        protected override void Handle()
        {
            //try
            //{
                using (var stream = new NetworkStream(socket))
                {
                    var packet = Serializer.DeserializeWithLengthPrefix<LdpPacket>(stream, PrefixStyle.Base128);
                    //base
                    Handle(packet);
                }
            //}
            /*catch (SocketException sockExc)
            {
                if (sockExc.ErrorCode == CLIENT_DISCON_ERROR_CODE)
                { 
                    // it's ok
                }
                else
                {
                    LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + sockExc.Message);
                }
            }
            catch (Exception ex)
            {
                LdpLog.Error("LdpPacketListener error: Handle() method throw:\n" + ex.Message);
                Restart();
            }*/
        }
    }
}
