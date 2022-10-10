using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChillLobbyServer
{
    internal class Message
    {
        public MyConnection thisConnection;
        public string myMessage;

        public Message(MyConnection thisConnection, string myMessage)
        {
            this.thisConnection = thisConnection;
            this.myMessage = myMessage;
        }
    }

    internal class AdminMessage
    {
        public string myMessage;

        public AdminMessage(string myMessage)
        {
            this.myMessage = myMessage;
        }
    }
}
