using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LdpThreads
{
    class LdpPacketListenerThread : LdpBaseThread
    {
        public LdpPacketListenerThread()
            : base("LdpPacketListenerThread")
        { }
        protected override void Run()
        {
            while (ThreadWorking)
            {
                MethodToStart();
            }
        }
    }
}
