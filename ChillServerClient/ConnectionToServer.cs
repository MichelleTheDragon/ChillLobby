using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChillServerClient
{
    internal class ConnectionToServer
    {
        TcpClient client = new TcpClient();
        IPEndPoint ep;

        public ConnectionToServer(string IpAddress)
        {
            ep = new IPEndPoint(IPAddress.Parse(IpAddress), 11000);
            client.Connect(ep);
        }
    }
}
