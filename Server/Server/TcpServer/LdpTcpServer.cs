using Server.Network;
using Server.Network.PacketHandler;
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

        #region Sockets variables
        private Socket serverSocket;
        private Socket clientSocket;
       
        private const int MAX_CONNECTIONS_PENDING = 5;
        private const int CLIENT_DISCON_ERROR_CODE = 10054;
        #endregion

        #region Server events
        public event ClientConnectedHandler OnClientConnected;
        public event ClientDisconnectedHandler OnClientDisconnected;
        public event ServerShutdownHandler OnServerShutdown;
        #endregion

        public LdpTcpServer(int port) 
        {
            this.port = port;
        }

        public LdpTcpServer()
        {
            this.port = 9998;
        }

        public void Start()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverIPEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(serverIPEP);
                serverSocket.Listen(MAX_CONNECTIONS_PENDING);
                LdpLog.Info(String.Format("Server started on: {0}:{1}.", GetServerIPAddress, port));

                clientSocket = serverSocket.Accept();
                if (OnClientConnected != null)
                    OnClientConnected(clientSocket.LocalEndPoint);
                LdpLog.Info("Client connected: " + clientSocket.LocalEndPoint);
                AddServerListener();

            }
            catch (SocketException sockExc)
            {
                if (sockExc.ErrorCode == CLIENT_DISCON_ERROR_CODE)
                {
                    if (OnClientDisconnected != null)
                        OnClientDisconnected();
                    LdpLog.Info("Client disconnected.");
                    Restart();
                }
                else
                {
                    LdpLog.Error(sockExc.Message);
                }
            }
            catch (Exception ex)
            {
                LdpLog.Error(ex.Message);
            }
        }

        //add packet listeners
        private void AddServerListener()
        {
            LdpPacketListener packetListener = new LdpPacketListener(clientSocket);
            packetListener.AddListener(new LdpAuthRequestHandler());
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
                const int SERVER_SHUTDOWN_TIMEOUT = 500;
                if (OnServerShutdown != null)
                    OnServerShutdown();

                if (serverSocket != null)
                {
                    LdpLog.Info("Server shutdown.");
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close(SERVER_SHUTDOWN_TIMEOUT);
                    serverSocket = null;
                    clientSocket = null;
                }
            }
            catch (SocketException sockExc)
            {
                LdpLog.Error("Server shutdown throw:\n" + sockExc.Message);
            }
            catch (Exception ex)
            {
                LdpLog.Error("Server shutdown throw:\n" + ex.Message);
            }
        }

        public string GetServerIPAddress
        {
            get 
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
                catch
                {
                    return "";
                }
            }
        }
    }
}
