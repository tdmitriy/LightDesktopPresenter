using Server.Network;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
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
    sealed class LdpPacketListener : LdpPacketHandler
    {
        private LdpServer serverHandler;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        private bool client_connected = false;
        private Thread listening_thread;
        public LdpPacketListener()
        {
            serverHandler = LdpServer.GetInstance();
            client_connected = true;
            listening_thread = new Thread(() => StartListening());
            listening_thread.Start();
            LdpLog.Info("Listener thread started.");
        }

        private void StartListening()
        {
            while (client_connected)
            {
                Handle();
            }
            LdpLog.Info("Listener thread stopped.");
        }

        protected override void Handle()
        {
            try
            {
                using (var stream = new NetworkStream(serverHandler.GetSocketChannel))
                {
                    var packet = LdpPacket.ParseDelimitedFrom(stream);

                    ProcessPacket(packet);
                    //base - notify to all subscribers
                    NotifyToAllListeners(packet);
                    //stream.Close();
                }
            }
            catch (IOException ioexc)
            {
                string error =
                            String.Format(@"LdpPacketListener IOException: 
                                    Handle() method throw:\n{0}.", ioexc.Message);
                LdpLog.Error(error);
                Restart();
            }
            catch (SocketException sockexc)
            {
                switch (sockexc.ErrorCode)
                {
                    case CLIENT_DISCON_ERROR_CODE:
                        LdpLog.Info("LdpPacketListener: client disconnected.");
                        Restart();
                        break;
                    default:
                        string error = 
                            String.Format(@"LdpPacketListener SocketException: 
                                    Handle() method throw:\n{0}.", sockexc.Message);
                        LdpLog.Error(error);
                        Restart();
                        break;
                }
            }
            catch (Exception ex)
            {
                string error =
                            String.Format(@"LdpPacketListener Exception: 
                                    Handle() method throw:\n{0}.", ex.Message);
                LdpLog.Error(error);
                Restart();
            }
        }
        private void ProcessPacket(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.DISCONNECT_REQUEST:
                    switch (packet.DisconnectRequest.Type)
                    {
                        case DisconnectionType.FROM_SERVER:
                            Restart();
                            break;
                    }
                    break;
            }
        }

        private void Restart()
        {
            
            AbortListenerThread();
            if (serverHandler != null)
                serverHandler.Restart();
        }

        private void AbortListenerThread()
        {
            client_connected = false;
            /*if (listening_thread != null)
                try
                {
                    listening_thread.Abort();
                    listening_thread = null;
                }
                catch { }*/
        }

        public override void Dispose()
        {
            base.Dispose();
            AbortListenerThread();
            serverHandler = null;
            GC.SuppressFinalize(this);
        }
    }
}
