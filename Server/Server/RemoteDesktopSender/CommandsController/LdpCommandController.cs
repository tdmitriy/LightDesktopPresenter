using Server.Network.Handlers.PacketHandlerBase;
using Server.Protocol;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteDesktopSender.CommandsController
{
    class LdpCommandController : ILdpPacketHandler, ILdpCommandController
    {
        private LdpServer serverHandler;
        public LdpCommandController()
        {
            serverHandler = LdpServer.GetInstance();
            serverHandler.GetListenerChannel.AddListener(this);
        }

        public void Handle(LdpPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.COMMAND:
                    var command = packet.Command;
                    ProcessCommand(command.Type);
                    break;
            }
        }

        public void ProcessCommand(CommandType type)
        {
            try
            {
                switch (type)
                {
                    /*case CommandType.SHOW_WINDOWS_KEYBOARD:
                        ShowWindowsKeyboard();
                        break;*/
                    case CommandType.REBOOT_PC:
                        serverHandler.DisconnectClient();
                        serverHandler.Stop();
                        Process.Start("shutdown", "/r /t 0");
                        break;
                    case CommandType.TURN_OFF_PC:
                        serverHandler.DisconnectClient();
                        serverHandler.Stop();
                        Process.Start("shutdown","/s /t 0");
                        break;
                }
            }
            catch
            {
                LdpLog.Error("Executing command error.");
            }
            
        }

        public void Dispose()
        {
            serverHandler = null;
        }
    }
}
