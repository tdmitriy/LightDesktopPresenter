using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LdpThreads
{
    interface ILdpBaseThread
    {
        void Start(Action methodToStart);
        void Start();
        void Stop();
    }
}
