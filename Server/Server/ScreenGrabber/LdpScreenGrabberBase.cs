using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ScreenGrabber
{
    abstract class LdpScreenGrabberBase : IDisposable
    {
        public abstract Bitmap GetScreenShot();

        public abstract void Dispose();
        
    }
}
