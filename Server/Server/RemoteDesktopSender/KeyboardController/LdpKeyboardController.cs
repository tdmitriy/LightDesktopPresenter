using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.RemoteDesktopSender.KeyboardController
{
    class LdpKeyboardController : ILdpPacketHandler, ILdpKeyboardController
    {
        private LdpServer serverHandler;
        public LdpKeyboardController()
        {
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
        }
        public void SendKey(string key, KeyboardKey type)
        {
            try
            {
                switch (type)
                {
                    case KeyboardKey.KEY_TEXT:
                        Send(key);
                        break;
                    case KeyboardKey.KEY_ENTER:
                        SendKeys.SendWait(LdpKeys.ENTER);
                        break;
                    case KeyboardKey.KEY_DEL:
                        SendKeys.SendWait(LdpKeys.BACKSPACE);
                        break;
                }

            }
            catch
            {
                LdpLog.Error("SendKeys unhandled key: " + key);
            }
        }

        private void Send(string key)
        {
            try
            {
                SendKeys.SendWait(key);
            }
            catch
            {
                try
                {
                    SendKeys.SendWait("{" + key + "}");
                }
                catch { }
            }

        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.KEYBOARD_INFO_RESPONSE:
                    ProcessPacket(packet);
                    break;
            }
        }

        private void ProcessPacket(LdpPacket packet)
        {
            var keyboardInfo = packet.KeyboardInfoResponse;
            string key = keyboardInfo.Key;
            KeyboardKey type = keyboardInfo.Type;
            SendKey(key, type);
        }

        public void Dispose()
        {
            serverHandler = null;
        }
    }
}
