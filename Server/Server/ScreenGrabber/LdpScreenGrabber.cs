using Server.Network;
using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ScreenGrabber
{
    // impl Factory pattern
    class LdpScreenGrabber : IDisposable
    {
        private static bool WINDOWS7 = LdpUtils.IsWindows7;
        private static bool WINDOWS8 = LdpUtils.IsWindows8;

        private LdpScreenGrabberBase screenGrabber;
        private LdpScreenGrabberType type;
        public LdpScreenGrabber()
        {
            LoadScreenGrabber();
        }

        private void LoadScreenGrabber()
        {
            if (WINDOWS7)
            {
                type = LdpScreenGrabberType.DIRECTX_9;
                InitScreenGrabber(type);
            }
            else if (WINDOWS8)
            {
                type = LdpScreenGrabberType.DIRECTX_11;
                InitScreenGrabber(type);
            }
        }

        private void InitScreenGrabber(LdpScreenGrabberType type)
        {
            switch (type)
            {
                case LdpScreenGrabberType.DIRECTX_11:
                    screenGrabber = new LdpDirectx11Grabber();
                    //screenGrabber = new LdpDirectx9Grabber();
                    break;
                case LdpScreenGrabberType.DIRECTX_9:
                    screenGrabber = new LdpDirectx9Grabber();
                    break;
            }
        }

        public Bitmap GetScreenShot()
        {
            return screenGrabber.GetScreenShot();
        }

        public void Dispose()
        {
            screenGrabber.Dispose();
        }
    }
}
