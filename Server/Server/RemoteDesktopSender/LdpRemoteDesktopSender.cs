using Server.Network;
using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.ScreenGrabber;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ4;
using System.Threading;
using Server.RemoteDesktopSender.MouseController;
using Server.RemoteDesktopSender.KeyboardController;
using Server.RemoteDesktopSender.CommandsController;

namespace Server.RemoteDesktopSender
{
    class LdpRemoteDesktopSender : ILdpPacketHandler
    {
        #region Variables
        private LdpServer serverHandler;
        private LdpScreenResponse screenResponse;
        private LdpProtocolPacketFactory packetFactory;
        private LdpScreenGrabber screenGrabber;
        private LdpPacket responsePacket;

        private Bitmap nextScreen;
        private Bitmap prevScreen;
        private Bitmap difference;

        private static readonly int SCREEN_WIDTH = LdpUtils.SCREEN_WIDTH;
        private static readonly int SCREEN_HEIGHT = LdpUtils.SCREEN_HEIGHT;

        private bool interruptGrabbing = false;
        private static readonly object LOCK = new object();
        private int baseLenght;
        private byte[] origData, compressed;
        private LdpRectangle.Builder rect;
        #endregion


        private LdpMouseController mouseController;
        private LdpKeyboardController keyboardController;
        private LdpCommandController commander;
        private static Thread ScreenThread;
        public LdpRemoteDesktopSender()
        {
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
            mouseController = new LdpMouseController();
            keyboardController = new LdpKeyboardController();
            commander = new LdpCommandController();

            packetFactory = new LdpProtocolPacketFactory();
            screenGrabber = new LdpScreenGrabber();
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.SCREEN_REQUEST:
                        /*ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
                        {
                            GetScreen();
                        }));*/
                    //if (ScreenThread != null && ScreenThread.IsAlive)
                        //ScreenThread.Join();
                    ScreenThread = new Thread(() => GetScreen());
                    ScreenThread.Start();
                    break;
                case PacketType.DISCONNECT_REQUEST:
                    var result = packet.DisconnectRequest;
                    switch (result.Type)
                    {
                        case DisconnectionType.FROM_SCREEN_THREAD:
                            interruptGrabbing = false;
                            LdpLog.Info("Exiting screen thread.");
                            serverHandler.GetListenerChannel.RemoveListener(this);
                            break;
                    }
                    break;
            }
        }

        private void SendScreenResponse(byte[] compr, int len, LdpRectangle.Builder r)
        {
            screenResponse = null;
            responsePacket = null;
            screenResponse = packetFactory.SetScreenResponse(compr, len, r);
            responsePacket = packetFactory.BuildPacket(screenResponse);
            serverHandler.GetSenderChannel.Send(responsePacket);
        }

        private void CheckRectangleBounds(ref Rectangle bounds)
        {
            if (bounds.Width > SCREEN_WIDTH) bounds.Width = SCREEN_WIDTH;
            if (bounds.Height > SCREEN_HEIGHT) bounds.Height = SCREEN_HEIGHT;
            if (bounds.Width < 0) bounds.Width = 0;
            if (bounds.Height < 0) bounds.Height = 0;
            if (bounds.X < 0) bounds.X = 0;
            if (bounds.Y < 0) bounds.Y = 0;
            if (bounds.X > SCREEN_WIDTH) bounds.X = SCREEN_WIDTH;
            if (bounds.Y > SCREEN_HEIGHT) bounds.Y = SCREEN_HEIGHT;
        }

        private void GetScreen()
        {
            lock (LOCK)
            {
                try
                {
                    Rectangle bounds = Rectangle.Empty;
                    interruptGrabbing = false;

                    while (!interruptGrabbing)
                    {

                        nextScreen = screenGrabber.GetScreenShot();
                        if (nextScreen == null) break;
                        bounds = LdpScreenProcessingUtils.GetBoundingBoxForChanges(nextScreen, prevScreen);

                        if (bounds != Rectangle.Empty)
                        {
                            interruptGrabbing = true;
                            CheckRectangleBounds(ref bounds);
                            difference = LdpScreenProcessingUtils.CopyRegion(nextScreen, bounds);

                            origData = LdpScreenProcessingUtils.ImageToByteArray(difference);
                            baseLenght = origData.Length;
                            compressed = LZ4Codec.Encode(origData, 0, baseLenght);

                            rect = LdpRectangle.CreateBuilder();
                            rect.Left = bounds.Left;
                            rect.Top = bounds.Top;
                            rect.Right = bounds.Right;
                            rect.Bottom = bounds.Bottom;

                            SendScreenResponse(compressed, baseLenght, rect);

                            difference.Dispose();
                            difference = null;
                            rect = null;
                            origData = null;
                            compressed = null;
                        }
                        prevScreen = nextScreen;
                    }
                }
                catch (Exception ex)
                {
                    LdpLog.Error("GetScreen thrown:\n" + ex.Message);
                }
            }
        }

        public void Dispose()
        {
            interruptGrabbing = false;
            serverHandler = null;
            screenResponse = null;
            packetFactory = null;
            responsePacket = null;

            try
            {
                if (ScreenThread != null && ScreenThread.IsAlive)
                    ScreenThread.Abort();
            }
            catch { }
            mouseController.Dispose();
            mouseController = null;

            keyboardController.Dispose();
            keyboardController = null;

            commander.Dispose();
            commander = null;

            screenGrabber.Dispose();
            screenGrabber = null;

            if (nextScreen != null)
                nextScreen.Dispose();
            if (prevScreen != null)
                prevScreen.Dispose();
            if (difference != null)
                difference.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
