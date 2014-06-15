using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Server.RemoteDesktopSender
{
    class LdpMouseController : ILdpPacketHandler, ILdpMouseController
    {
        #region WinApi Mouse Func
        [Flags]
        public enum MouseFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy,
            int dwData, int dwExtraInfo);

        #endregion

        #region Mouse variables
        private int MouseX { get; set; }
        private int MouseY { get; set; }
        #endregion

        private LdpServer serverHandler;
        public LdpMouseController()
        {
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.MOUSE_INFO_RESPONSE:
                    ProcessPacket(packet);
                    break;
            }
        }

        private void ProcessPacket(LdpPacket packet)
        {
            var mouseResponse = packet.MouseInfoResponse;
            MouseX = mouseResponse.X;
            MouseY = mouseResponse.Y;

            switch (packet.MouseInfoResponse.Type)
            {
                case MouseType.SET_CURSOR_POS:
                    SetCursorPosition(MouseX, MouseY);
                    break;
                case MouseType.LEFT_CLICK:
                    MouseLeftClick();
                    break;
                case MouseType.LEFT_DOUBLE_CLICK:
                    MouseDoubleClick();
                    break;
                case MouseType.RIGHT_CLICK:
                    MouseRightClick();
                    break;
            }
        }

        #region Mouse Clicks
        public void MouseLeftClick()
        {
            MousePoint position = GetCursorPosition();

            mouse_event((int)MouseFlags.LeftDown | (int)MouseFlags.LeftUp,
                position.X, position.Y, 0, 0);
        }

        public void MouseRightClick()
        {
            MousePoint position = GetCursorPosition();

            mouse_event((int)MouseFlags.RightDown | (int)MouseFlags.RightUp,
                position.X, position.Y, 0, 0);
        }

        public void MouseDoubleClick()
        {
            MousePoint position = GetCursorPosition();

            mouse_event((int)MouseFlags.LeftDown | (int)MouseFlags.LeftUp,
                position.X, position.Y, 0, 0);

            mouse_event((int)MouseFlags.LeftDown | (int)MouseFlags.LeftUp,
                position.X, position.Y, 0, 0);
        }
        #endregion

        public void SetCursorPosition(int x, int y)
        {
            //SetCursorPos(x, y);
            System.Windows.Forms.Cursor.Position = 
                new System.Drawing.Point(x, y);
        }

        public void SetCursorPosition(MousePoint point)
        {
            //SetCursorPos(point.X, point.Y);
            System.Windows.Forms.Cursor.Position = 
                new System.Drawing.Point(point.X, point.Y);
        }

        public MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public void Dispose()
        {
            serverHandler = null;
        }
    }
}
