using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteDesktopSender.KeyboardController
{
    interface ILdpKeyboardController
    {
        void SendKey(string text, KeyboardKey type);
    }
}
