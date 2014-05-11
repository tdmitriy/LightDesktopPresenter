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

namespace Server.Network.PacketSender
{
    class LdpPacketSender : ILdpPacketSender
    {
        private ILdpTcpServer serverHandler;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        public LdpPacketSender(ILdpTcpServer serverHandler)
        {
            this.serverHandler = serverHandler;
        }
        public void Send(LdpPacket packet)
        {
            try
            {
                using (var stream = new NetworkStream(serverHandler.ClientChannel))
                {
                    Serializer.SerializeWithLengthPrefix<LdpPacket>(stream, packet, PrefixStyle.Base128);
                }
            }
            catch (IOException ioexc)
            {
                LdpLog.Error("Send packet error:\n" + ioexc.Message);
            }
            catch (SocketException sockExc)
            {
                if (sockExc.ErrorCode == CLIENT_DISCON_ERROR_CODE)
                {
                    LdpLog.Info("Client disconnected.");
                    serverHandler.Restart();
                }
                else
                {
                    LdpLog.Error(sockExc.Message);
                }
            }
            catch (Exception ex)
            {
                LdpLog.Error("Send packet error:\n" + ex.Message);
            }
            
        }
    }
}
