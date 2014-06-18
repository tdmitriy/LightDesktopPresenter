using Server.ClientInfo;
using Server.Network.Handlers;
using Server.TcpServer.PacketListener;
using Server.TcpServer.PacketSender;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    abstract class LdpServerBase : LdpServerInitializator, ILdpServer
    {
        private const int DEFAULT_PORT_NUMBER = 9998;
        private int PORT { get; set; }

        
        public void Start(int port)
        {
            try
            {
                this.PORT = port;
                base.StartServer(port);
            }
            catch (Exception ex)
            {
                LdpLog.Error("Starting server error:\n" + ex.Message);
                Stop();
            }
        }

        public void Start()
        {
            Start(DEFAULT_PORT_NUMBER);
        }

        public void Restart()
        {
            LdpLog.Info("Restarting the server.");
            base.StopServer();
            base.StartServer(DEFAULT_PORT_NUMBER);
        }

        public void Stop()
        {
            base.StopServer();
        }

        public void DisconnectClient()
        {
            base.DisconntectClientFromServer();
        }

        public int GetServerPort
        {
            get { return PORT; }
        }

        public string GetClientIPAddress
        {
            get { return GetClientIpAddress(); }
        }

        public LdpPacketListener GetListenerChannel
        {
            get { return packetListener; }
        }

        public LdpPacketSender GetSenderChannel
        {
            get { return packetSender; }
        }

        public Socket GetSocketChannel
        {
            get { return clientSocket; }
        }

        public new List<string> GetLocalIpAddressList
        {
            get { return GetLocalIpAddressList(); }
        }


        
    }
}
