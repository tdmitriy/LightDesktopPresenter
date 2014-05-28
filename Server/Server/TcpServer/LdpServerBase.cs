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
    abstract class LdpServerBase : ILdpServer
    {
        private int port { get; set; }
        private const int DEFAULT_PORT_NUMBER = 9998;

        #region Sockets variables
        private Socket serverSocket;
        private Socket clientSocket;

        private const int MAX_CONNECTIONS_PENDING = 5;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        private const int SERVER_CLOSING_CODE = 10004;
        private Thread connectionThread;
        #endregion


        private LdpPacketSender packetSender;
        private LdpPacketListener packetListener;
        private string clientIP;
        private IPEndPoint clientEndPoint;

        private LdpAuthRequestHandler authRequestHandler;
        private LdpClientInfoRequestHandler clientInfoRequestHandler;
        private LdpPreparableInfoRequestHandler preparableInfoRequestHandler;
        private LdpDisconnectRequestHandler disconnectionHandler;


        public virtual void Start(int port)
        {
            try
            {
                this.port = port;
                serverSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverIPEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(serverIPEP);
                serverSocket.Listen(MAX_CONNECTIONS_PENDING);
                LdpLog.Info(String.Format("Server started on: {0}:{1}.",
                    GetServerIPAddress, GetServerPort));

                //incoming connection
                clientSocket = serverSocket.Accept();
                clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
                clientIP = clientEndPoint.Address.ToString();

                LdpLog.Info(String.Format("Incoming connection: {0}.\nWaiting auth request..",
                    clientSocket.LocalEndPoint));
                AddServerHandlers();

            }
            catch (SocketException sockexc)
            {
                switch (sockexc.ErrorCode)
                {
                    //closing server
                    case SERVER_CLOSING_CODE:
                        LdpLog.Info("Closing server.");
                        Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                LdpLog.Error(String.Format("Starting server error:\n{0}",
                    ex.Message));
                Stop();
            }
        }

        public virtual void Start()
        {
            connectionThread = new Thread(() =>
            {
                Start(DEFAULT_PORT_NUMBER);
            });
            connectionThread.Start();
        }

        private void AddServerHandlers()
        {
            packetSender = new LdpPacketSender();
            packetListener = new LdpPacketListener();

            authRequestHandler = new LdpAuthRequestHandler();
            clientInfoRequestHandler = new LdpClientInfoRequestHandler();
            preparableInfoRequestHandler = new LdpPreparableInfoRequestHandler();
            disconnectionHandler = new LdpDisconnectRequestHandler();

            packetListener.AddListener(authRequestHandler);
            packetListener.AddListener(clientInfoRequestHandler);
            packetListener.AddListener(preparableInfoRequestHandler);
            packetListener.AddListener(disconnectionHandler);
        }

        public virtual void Stop()
        {

            ClearClientInfo();
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    clientSocket.Dispose();
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
            finally
            {
                clientSocket = null;
            }

            try
            {
                if (serverSocket != null)
                {
                    LdpLog.Info("Server shutdown.");
                    serverSocket.Close();
                    serverSocket.Dispose();
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
            finally
            {
                serverSocket = null;
            }


            if (packetListener != null)
            {
                authRequestHandler.Dispose();
                clientInfoRequestHandler.Dispose();
                preparableInfoRequestHandler.Dispose();
                disconnectionHandler.Dispose();
                packetListener.RemoveListeners();
                packetListener.Dispose();
            }
            packetListener = null;
            packetSender = null;

            try
            {
                if (connectionThread != null && connectionThread.IsAlive)
                {
                    connectionThread.Abort();
                }
            }
            catch (Exception sxc)
            {
                LdpLog.Error("connectionThread interrupt error.\n" + sxc.Message);
            }

        }

        private void ClearClientInfo()
        {
            LdpClientInfo.IP = "";
            LdpClientInfo.DEVICE_NAME = "";
            LdpClientInfo.OS = "";
        }

        public virtual void Restart()
        {
            LdpLog.Info("Restarting server.");
            Stop();
            Start(port);
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

        public string GetServerIPAddress
        {
            get { return serverIPAddress(); }
        }

        public int GetServerPort
        {
            get { return port; }
        }

        public string GetClientIPAddress
        {
            get { return clientIP; }
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
    }
}
