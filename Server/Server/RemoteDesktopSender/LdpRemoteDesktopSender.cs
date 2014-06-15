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

        private Thread ScreenProcessing;
        private static readonly object LOCK = new object();
        private int baseLenght;
        private byte[] origData, compressed;
        private LdpRectangle.Builder rect;
        private static readonly int SLEEP_TIME = 10;
        #endregion

        private LdpMouseController mouseController;
        public LdpRemoteDesktopSender()
        {
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
            mouseController = new LdpMouseController();

            packetFactory = new LdpProtocolPacketFactory();
            screenGrabber = new LdpScreenGrabber();
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.SCREEN_REQUEST:
                    ScreenProcessing = new Thread(() => GetScreen());
                    ScreenProcessing.Start();
                    break;
                case PacketType.DISCONNECT_REQUEST:
                    var result = packet.DisconnectRequest;
                    switch (result.Type)
                    {
                        case DisconnectionType.FROM_SCREEN_THREAD:
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
                Rectangle bounds = Rectangle.Empty;

                while (bounds == Rectangle.Empty)
                {
                    nextScreen = screenGrabber.GetScreenShot();
                    bounds = LdpScreenProcessingUtils.GetBoundingBoxForChanges(nextScreen, prevScreen);

                    if (bounds != Rectangle.Empty)
                    {
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
                    try
                    {
                        Thread.Sleep(SLEEP_TIME);
                    }
                    catch { }
                }
            }
        }

        public void Dispose()
        {
            serverHandler = null;
            screenResponse = null;
            packetFactory = null;
            responsePacket = null;

            try
            {
                if (ScreenProcessing != null)
                    ScreenProcessing.Abort();
            }
            catch { }
           
            mouseController.Dispose();
            mouseController = null;

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
