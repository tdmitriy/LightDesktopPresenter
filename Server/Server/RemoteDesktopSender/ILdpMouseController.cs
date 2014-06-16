using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.RemoteDesktopSender
{
    interface ILdpMouseController
    {
        void MouseLeftClick();
        void MouseRightClick();
        void MouseDoubleClick();
        void MouseLeftBtnDown();
        void MouseLeftBtnUp();
        void SetCursorPosition(int x, int y);
        void SetCursorPosition(MousePoint point);
        MousePoint GetCursorPosition();
    }
}
