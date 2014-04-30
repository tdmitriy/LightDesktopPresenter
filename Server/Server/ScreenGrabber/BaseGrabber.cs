using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Server.ScreenGrabber
{
    abstract class BaseGrabber
    {
        protected abstract void InitializeDX9();
        protected abstract void InitializeDX11();
        protected abstract Bitmap GetDX9ScreenShot();
        protected abstract Bitmap GetDX11ScreenShot();
        public abstract Bitmap GetScreenShot();
    }
}
