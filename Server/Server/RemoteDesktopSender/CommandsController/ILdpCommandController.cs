using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteDesktopSender.CommandsController
{
    interface ILdpCommandController
    {
        void ProcessCommand(CommandType type);
    }
}
