using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Server.WindowsUtils;
using Server.TcpServer;
using Server.Network;
using Server.Protocol;

namespace Server.TcpServer.PacketSender
{
    class LdpPacketSender : ILdpPacketSender, IDisposable
    {
        private LdpServer serverHandler = LdpServer.GetInstance();
        public void Send(LdpPacket packet)
        {
            try
            {
                using (var stream = new NetworkStream(serverHandler.GetSocketChannel))
                {
                    packet.WriteDelimitedTo(stream);
                }
            }
            catch (IOException ioexc)
            {
                LdpLog.Error("Send packet error:\n" + ioexc.Message);
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("Send packet error:\n" + sockexc.Message);
            }
            catch (Exception ex)
            {
                LdpLog.Error("Send packet error:\n" + ex.Message);
            }
        }

        public void Dispose()
        {
            serverHandler = null;
        }
    }
}
