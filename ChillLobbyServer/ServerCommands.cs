using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChillLobbyServer
{
    internal class ServerCommands
    {
        static int port = 11000;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        List<MyConnection> allConnections = new List<MyConnection>();
        List<Message> allMessages = new List<Message>();
        List<Message> newMessages = new List<Message>();
        List<AdminMessage> allAdminMessages = new List<AdminMessage>();
        object msgLock = new object();
        bool serverLive = false;

        public void StartServer()
        {
            server.Start();
            serverLive = true;
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
            Console.WriteLine("Someone joined!");
            MyConnection clientCon = new MyConnection(client, "Player");
            allConnections.Add(clientCon);
            NetworkStream stream = client.GetStream();
            InboundPackages(stream, clientCon);
        }

        public void StopServer()
        {
            serverLive = false;
            server.Stop();
            Console.WriteLine("Server Successfully shut down.");
        }

        public void InboundPackages(NetworkStream client, MyConnection clientCon)
        {
            try
            {
                while (serverLive)
                {
                    byte[] msg = new byte[1024];
                    client.Read(msg, 0, msg.Length);
                    string dataDecoded = Encoding.UTF8.GetString(msg);

                    Console.WriteLine("User (" + clientCon.myConnection.ToString() + ") wrote: " + dataDecoded);

                    lock (msgLock)
                    {
                        newMessages.Add(new Message(clientCon, dataDecoded));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OutboundPackages()
        {
            while (serverLive)
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
                                byte[] encodedMsg = Encoding.UTF8.GetBytes(m.myMessage);
                                c.myConnection.GetStream().Write(encodedMsg, 0, encodedMsg.Length);
                            }
                        }
                    }
                }
            }
        }

        public void ConsoleCommands()
        {
            while (serverLive)
            {
                string commandText = Console.ReadLine() + "";
                if (commandText[0] != '!')
                {
                    Console.WriteLine("No command entered (Type \"!Help\" to see list of commands)");
                }
                else
                {
                    switch (commandText)
                    {
                        case "!Help": case "!help":
                            Console.WriteLine("    Command     |        Example        |       Description");
                            Console.WriteLine("!Help            \"!Help\"                 Shows the list of server commands");
                            Console.WriteLine("!IP              \"!IP\"                   Shows the servers IP address");
                            Console.WriteLine("!Msg             \"!Msg Hello World\"      Writes server messages to the clients");
                            Console.WriteLine("!Save            \"!Save World1\"          Creates a save file of the current game state");
                            Console.WriteLine("!Load            \"!Load World1\"          Loads a save file if it exists");
                            Console.WriteLine("!Close           \"!Close\"                Shuts down the server");
                            break;
                        case "!Msg": case "!msg":
                            if (commandText.Length >= 6)
                            {
                                allAdminMessages.Add(new AdminMessage(commandText.Substring(6, commandText.Length)));
                            }
                            else
                            {
                                Console.WriteLine("No message to send");
                            }
                            break;
                        case "!IP": case "!Ip": case "!ip":
                            //Get ip
                            string hostName = Dns.GetHostName();
                            string myIP = Dns.GetHostEntry(hostName).AddressList[3].ToString();
                            Console.WriteLine("Server IP Address: \"" + myIP + ":" + port + "\"");
                            break;
                        case "!Clients": case "!clients":
                            Console.WriteLine("Client number  |    Client Ip Address    |    Client Name");
                            for (int i = 0; i < allConnections.Count; i++)//MyConnection c in allConnections)
                            {
                                Console.WriteLine(i+1 + "        " + allConnections[i].myConnection.ToString() + "      " + allConnections[i].name);
                            }
                            break;
                        case "!Save": case "!save":
                            if (commandText.Length >= 7)
                            {
                                string saveName = commandText.Substring(7, commandText.Length);
                                bool attemptSave = SaveCommand(saveName);
                                if (attemptSave == true)
                                {
                                    Console.WriteLine("File saved with name: " + saveName);
                                }
                                else
                                {
                                    Console.WriteLine("Failed to save file with name: " + saveName);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No name was given to the savefile, try again (Example: \"!Save Test\")");
                            }
                            break;
                        case "!Load": case "!load":
                            if (commandText.Length >= 7)
                            {
                                string loadName = commandText.Substring(7, commandText.Length);
                                bool attemptLoad = LoadCommand(loadName);
                                if (attemptLoad == true)
                                {
                                    Console.WriteLine($"Loading file \"{loadName}\" now...");
                                }
                                else
                                {
                                    Console.WriteLine($"The file \"{loadName}\" was not found");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No file name was given");
                            }
                            break;
                        case "!Close": case "!close":
                            StopServer();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void UpdateAsync()
        {

        }

        public bool SaveCommand(string saveFileName)
        {
            return false;
        }

        public bool LoadCommand(string loadFileName)
        {
            return false;
        }
    }
}
