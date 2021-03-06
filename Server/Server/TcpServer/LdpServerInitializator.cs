﻿using Server.ClientInfo;
using Server.Network;
using Server.Network.Handlers;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.RemoteDesktopSender;
using Server.RemoteVolumeSender;
using Server.TcpServer.PacketListener;
using Server.TcpServer.PacketSender;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.TcpServer
{
    abstract class LdpServerInitializator
    {
        #region Socket variables
        protected Socket clientSocket;
        private Socket serverSocket;
        private IPEndPoint clientEndPoint;

        private string clientIP;
        private const int MAX_CONNECTIONS_PENDING = 5;
        private const int SERVER_CLOSING_CODE = 10004;

        private Thread serverThread;
        #endregion

        protected LdpPacketSender packetSender { get; private set; }
        protected LdpPacketListener packetListener { get; private set; }

        private LdpAuthRequestHandler authRequestHandler;
        private LdpDisconnectRequestHandler disconnectHandler;

        private void InitializeServer(int port)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverIPEP = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(serverIPEP);
                serverSocket.Listen(MAX_CONNECTIONS_PENDING);

                PrintServerStarting();

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
                        StopServer();
                        break;
                    default:
                        LdpLog.Error("Starting server error:\n" + sockexc.Message);
                        StopServer();
                        break;
                }
            }
            
        }

        protected void StartServer(int port)
        {
            try
            {
                if (serverThread != null && serverThread.IsAlive)
                {
                    serverThread.Abort();
                    serverThread = null;
                }
            }
            catch { }
            serverThread = new Thread(() => InitializeServer(port));
            serverThread.Start();
        }

        private void AddServerHandlers()
        {
            packetListener = new LdpPacketListener();
            packetSender = new LdpPacketSender();

            authRequestHandler = new LdpAuthRequestHandler();
            disconnectHandler = new LdpDisconnectRequestHandler();
                
            packetListener.AddListener(authRequestHandler);
            packetListener.AddListener(disconnectHandler);
        }

        protected void StopServer()
        {
            try
            {
                Cleanup();
                if (serverSocket != null)
                {
                    LdpLog.Info("Server shutdown.");
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        clientSocket.Dispose();
                    }
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
                clientSocket = null;
                serverSocket = null;
            }

            try
            {
                if (serverThread != null)
                {
                    serverThread.Abort();
                    serverThread = null;
                }
                    
            }
            catch { LdpLog.Error("ServerThread interrupted."); }
            GC.Collect();
        }

        protected void DisconntectClientFromServer()
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    var factory = new LdpProtocolPacketFactory();
                    var list = packetListener.GetListenersList;
                    LdpDisconnectRequest disconnectPacket = null;
                    LdpPacket packet = null;
                    const int TIMEOUT = 100;
                    if (list != null && list.Count != 0)
                    {
                        foreach (var item in list)
                        {
                            if (item is LdpRemoteDesktopSender)
                            {
                                disconnectPacket = factory
                                    .SetDisconnectRequest(DisconnectionType.FROM_SCREEN_THREAD);
                                packet = factory.BuildPacket(disconnectPacket);
                                packetSender.Send(packet);
                                Thread.Sleep(TIMEOUT);
                                break;
                            }
                            else if (item is LdpRemoteVolumeSender)
                            {
                                disconnectPacket = factory
                                    .SetDisconnectRequest(DisconnectionType.FROM_VOLUME_THREAD);
                                packet = factory.BuildPacket(disconnectPacket);
                                packetSender.Send(packet);
                                Thread.Sleep(TIMEOUT);
                                break;
                            }
                        }
                    }

                    disconnectPacket = factory
                                    .SetDisconnectRequest(DisconnectionType.FROM_SERVER);
                    packet = factory.BuildPacket(disconnectPacket);
                    packetSender.Send(packet);
                    disconnectPacket = null;
                    packet = null;
                }
                catch(Exception exc)
                {
                    LdpLog.Error("DisconntectClient error.\n" + exc.Message);
                }
            }
        }

        private void PrintServerStarting()
        {
            string text = "Try to use one of the following IP:\n";
            foreach (var ip in GetLocalIpAddressList())
            {
                text += ip + "\n";
            }
            LdpLog.Info(text);
            LdpLabelStatus.GetInstance().StateText = text;
        }

        private void Cleanup()
        {
            ClearClientInfo();

            if (packetListener != null)
            {
                packetListener.Dispose();
                packetListener = null;
            }

            if (packetSender != null)
            {
                packetSender.Dispose();
                packetSender = null;
            }
        }

        private void ClearClientInfo()
        {
            LdpClientInfo.IP = "";
            LdpClientInfo.DEVICE_NAME = "";
            LdpClientInfo.OS = "";
        }

        protected List<string> GetLocalIpAddressList()
        {
            var tmp = new List<string>();
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        tmp.Add(ip.ToString());
                    }
                }
                return tmp;
            }
            catch (SocketException sockexc)
            {
                LdpLog.Error("Getting IP address list error:\n" + sockexc.Message);
                return new List<string>();
            }
        }

        protected string GetClientIpAddress()
        {
            return clientIP;
        }
    }
}
