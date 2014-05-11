using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security;

namespace Server.AbstractThread
{
    abstract class LdpBaseThread : ILdpBaseThread
    {
        //TODO
        private static Thread thread;
        protected abstract bool threadWorking;
        protected abstract void Run();
        

        public static void StopThread()
        {
            
        }

        public void Start()
        {
            threadWorking = true;
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private static void ThreadStop()
        {

        }

        public void Stop()
        {
            if (thread != null && thread.IsAlive && threadWorking)
            {
                try
                {
                    threadWorking = true;
                    thread.Interrupt();
                }
                catch { }
            }
        }
    }
}
