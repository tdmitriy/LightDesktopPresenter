using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Server.ScreenGrabber;

namespace Server.AbstractThread
{
    class LdpScreenThread : LdpBaseThread
    {
        private LdpDirectxGrabber dxGrabber;
        public LdpScreenThread()
        {
            dxGrabber = new LdpDirectxGrabber();
        }

        protected override void Run()
        {
            //screen grabbing..
            
        }

    }
}
