using Server.WindowsUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.LdpThreads
{
    class LdpVolumeThread : LdpBaseThread
    {
        public LdpVolumeThread()
            : base("LdpVolumeThread")
        {
            //workingThread = new Thread(new ThreadStart(Run));
            //workingThread.Name = "LdpVolumeThread";
        }

        protected override void Run()
        {
            while (ThreadWorking)
            {

            }
        }


        public void StopStatic()
        {
            //Stop();
        }
    }
}
