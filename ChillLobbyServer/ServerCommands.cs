using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChillLobbyServer
{
    internal class ServerCommands
    {
        static int port = 11000;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        List<MyConnection> allConnections = new List<MyConnection>();
        List<Message> allMessages = new List<Message>();
        List<Message> newMessages = new List<Message>();
        object msgLock = new object();

        public void StartServer()
        {
            server.Start();
            AcceptConnection();
        }

        private void AcceptConnection()
        {
            server.BeginAcceptTcpClient(HandleConnection, server);
        }

        private void HandleConnection(IAsyncResult result)
        {
            AcceptConnection();
            TcpClient client = server.EndAcceptTcpClient(result);
            MyConnection clientCon = new MyConnection(client, "");
            allConnections.Add(clientCon);
            NetworkStream stream = client.GetStream();
            InboundPackages(stream, clientCon);
        }

        public void StopServer()
        {
            server.Stop();
        }

        public void InboundPackages(NetworkStream client, MyConnection clientCon)
        {
            try
            {
                while (true)
                {
                    byte[] msg = new byte[1024];
                    client.Read(msg, 0, msg.Length);
                    var dataDecoded = Encoding.UTF8.GetString(msg);

                    newMessages.Add(new Message(clientCon, dataDecoded));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OutboundPackages()
        {
            while (true)
            {
                if (newMessages.Count > 0)
                {
                    lock (msgLock)
                    {
                        allMessages = new List<Message>(newMessages);
                        newMessages.Clear();
                    }
                    foreach (MyConnection c in allConnections) {
                        if (c.myConnection.Connected == true)
                        {
                            foreach (Message m in allMessages)
                            {

                            }
                        }
                    }
                }
            }
        }

        public void ConsoleCommands()
        {

        }

        public void UpdateAsync()
        {

        }
    }
}
