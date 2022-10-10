﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChillLobbyServer
{
    internal class MyConnection
    {
        public TcpClient myConnection;
        public IPEndPoint myIp;
        public Player myPlayer;
        //public bool needsToRespond = false;
        //public bool hasResponded = false;
        public string name = "";
        public int objectId = 0; //0 = nothing

        public MyConnection(TcpClient myConnection, string name)
        {
            this.myConnection = myConnection;
            this.name = name;
            myIp = (IPEndPoint)myConnection.Client.RemoteEndPoint;
            Console.WriteLine(name + " (" + myIp.ToString() + ") has joined the server.");
            myPlayer = new Player();
        }
    }
}
