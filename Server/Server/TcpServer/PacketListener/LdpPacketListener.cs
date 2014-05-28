using ProtoBuf;
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
        }

        private void StartListening()
        {
            while (client_connected)
            {
                Handle();
            }
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
                switch (sockexc.ErrorCode)
                {
                    case CLIENT_DISCON_ERROR_CODE:
                        LdpLog.Info("LdpPacketListener: client diskonnected.");
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
                LdpLog.Error("LdpPacketListener Exception: Handle() method throw:\n" + ex.Message);
                Restart();
            }
        }
        private void Restart()
        {
            client_connected = false;
            if (serverHandler != null)
                serverHandler.Restart();
        }

        public override void Dispose()
        {
            base.Dispose();
            client_connected = false;
            serverHandler = null;
        }
    }
}
