using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;
using Server.WindowsUtils;
using Server.TcpServer;
using Server.Network;

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
                    Serializer.SerializeWithLengthPrefix<LdpPacket>(stream, packet, PrefixStyle.Base128);
                }
            }
            catch (IOException ioexc)
            {
                LdpLog.Error("Send packet error:\n" + ioexc.Message);
                Restart();
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("Send packet error:\n" + sockexc.Message);
                Restart();
            }
            catch (Exception ex)
            {
                LdpLog.Error("Send packet error:\n" + ex.Message);
                Restart();
            }
        }
        private void Restart()
        {
            serverHandler.Restart();
        }

        public void Dispose()
        {
            serverHandler = null;
        }
    }
}
