using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Server.ScreenGrabber;
using Server.WindowsUtils;
using Server.Network.PacketHandler;

namespace Server.LdpThreads
{
    class LdpScreenThread : LdpBaseThread
    {
        private LdpDirectxGrabber dxGrabber;
        private static bool worker = true;
        private static Thread staticThread;
        public LdpScreenThread()
            : base("LdpScreenThread")
        {
            ThreadWorking = worker;
            staticThread = workingThread;
            dxGrabber = new LdpDirectxGrabber();
        }

        protected override void Run()
        {
            int i = 0;
            while (ThreadWorking)
            {
                Console.WriteLine(i);
                Thread.Sleep(1000);
                i++;
            }
            LdpLog.Info("LdpScreenThread EXITING WHILE LOOP.");
        }

    }
}
