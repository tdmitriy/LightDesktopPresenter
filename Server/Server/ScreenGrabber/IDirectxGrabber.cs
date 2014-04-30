using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Server.ScreenGrabber
{
    interface IDirectxGrabber
    {
        void InitializeDX9();
        void InitializeDX11();
        Bitmap GetDX9Screenshot();
        Bitmap GetDX11Screenshot();
    }
}
