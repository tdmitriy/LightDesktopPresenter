using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class LdpTcpServer : ILdpServer
    {
        private int port;
        public LdpTcpServer(int port) 
        {
            this.port = port;
        }

        public void StartServer()
        {
            
        }

        public void StopServer()
        {
            
        }
    }
}
