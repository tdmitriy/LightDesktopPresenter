using Server.Network;
using Server.Network.PacketHandler;
using Server.Network.PacketSender;
using Server.TcpServer;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class LdpTcpServer : ILdpTcpServer
    {
        [DefaultValue(9998)]
        private int port { get; set; }
        private const int PORT_NUMBER = 9998;

        #region Sockets variables
        private Socket serverSocket;
        private Socket clientSocket;
       
        private const int MAX_CONNECTIONS_PENDING = 5;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        #endregion

        public LdpTcpServer(int port) 
        {
            this.port = port;
        }

        public LdpTcpServer()
        {
            this.port = PORT_NUMBER;
        }

        public void Start()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverIPEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(serverIPEP);
                serverSocket.Listen(MAX_CONNECTIONS_PENDING);
                LdpLog.Info(String.Format("Server started on: {0}:{1}.", 
                    GetServerIPAddress, GetServerPort));

                //incoming connection
                clientSocket = serverSocket.Accept();

                LdpLog.Info(String.Format("Incoming connection: {0}.\nWaiting auth request..",
                    clientSocket.LocalEndPoint));
                AddServerHandlers();

            }
            catch (SocketException sockexc)
            {
                if (sockexc.ErrorCode == CLIENT_DISCON_ERROR_CODE)
                    LdpLog.Info("Client disconnected.");
                else
                    LdpLog.Error(String.Format("Starting server error:\n{0}", 
                        sockexc.Message));
                Restart();
            }
            catch (Exception ex)
            {
                LdpLog.Error(String.Format("Starting server error:\n{0}", 
                    ex.Message));
            }
        }

        //add packet sender
        //add packet listeners
        private void AddServerHandlers()
        {
            LdpPacketSender packetSender = new LdpPacketSender(this);

            LdpPacketListener packetListener = 
                new LdpPacketListener(this, packetSender);
            packetListener.AddListener(new LdpAuthRequestHandler());
            packetListener.AddListener(new LdpClientInfoRequestHandler());
            packetListener.AddListener(new LdpDisconnectRequestHandler());
        }

        public void Restart()
        {
            LdpLog.Info("Restarting server.");
            Stop();
            Start();
            LdpLog.Info("Restarting server done.");
        }

        public void Stop()
        {
            try
            {
                const int SERVER_SHUTDOWN_TIMEOUT = 200;
                if (serverSocket != null)
                {
                    LdpLog.Info("Server shutdown.");
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close(SERVER_SHUTDOWN_TIMEOUT);
                    serverSocket = null;
                    clientSocket = null;
                }
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("Server shutdown thrown:\n" + sockexc.Message);
            }
            catch (Exception ex)
            {
                LdpLog.Error("Server shutdown thrown:\n" + ex.Message);
            }
        }

        private string serverIPAddress()
        {
            try
            {
                string localIP = "";
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                return localIP;
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("Getting IP address error:\n" + sockexc.Message);
                return "";
            }
        }

        public string GetServerIPAddress
        {
            get { return serverIPAddress(); }
        }

        public int GetServerPort
        {
            get { return port; }
        }

        public Socket ClientChannel
        {
            get { return clientSocket; }
        }
    }
}
