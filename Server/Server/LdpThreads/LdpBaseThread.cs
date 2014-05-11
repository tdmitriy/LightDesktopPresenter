using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security;
using Server.WindowsUtils;

namespace Server.LdpThreads
{
    abstract class LdpBaseThread : ILdpBaseThread
    {
        protected Thread workingThread;
        protected abstract void Run();
        protected bool ThreadWorking { get; set; }
        protected LdpBaseThread()
        {
            workingThread = new Thread(new ThreadStart(Run));
        }
        protected LdpBaseThread(string threadName)
        {
            //ThreadWorking = true;
            workingThread = new Thread(new ThreadStart(Run));
            workingThread.Name = threadName;
        }

        public bool IsThreadAlive
        {
            get { return ThreadWorking; }
        }

        public void Start()
        {
            
            workingThread.Start();
            LdpLog.Info(String.Format("Thread: {0} started.", workingThread.Name));
        }

        public void Stop()
        {
            if (workingThread != null && workingThread.IsAlive && ThreadWorking)
            {
                try
                {
                    ThreadWorking = false;
                    LdpLog.Info(String.Format("Thread: {0} is interrupted.", workingThread.Name));
                }
                catch { }
            }
        }
    }
}
