using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Server.ScreenGrabber;
using Server.WindowsUtils;

namespace Server.LdpThreads
{
    class LdpScreenThread : LdpBaseThread
    {
        private LdpDirectxGrabber dxGrabber;
        public LdpScreenThread()
            : base("LdpScreenThread")
        {
            dxGrabber = new LdpDirectxGrabber();
        }

        protected override void Run()
        {
            //grab screen
            /*while (ThreadWorking)
            {
                //MethodToStart();

            }*/
        }

    }
}
