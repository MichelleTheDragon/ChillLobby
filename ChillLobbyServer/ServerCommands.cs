using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection.Metadata;
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

        int outServerFull = 1;
        int outServerChange = 2;
        int outServerMsg = 3;
        int outClientMsg = 4;

        int inClientAction = 1;
        int inClientMsg = 2;

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
            NetworkStream stream = client.GetStream();
            string isValidUser = CheckUserAsync(stream).GetAwaiter().GetResult();
            if (isValidUser.Length <= 0)
            {
                client.Close();
            }
            else
            {
                MyConnection clientCon = new MyConnection(client, isValidUser);
                allConnections.Add(clientCon);
                InboundPackages(stream, clientCon);
            }
    }
        private static async Task<string> CheckUserAsync(NetworkStream stream)
        {
            byte[] msg = new byte[1024];
            int bytes = stream.Read(msg, 0, msg.Length);
            string dataDecoded = Encoding.UTF8.GetString(msg, 0, bytes);
            string[] splitInfo = dataDecoded.Split(":-:SplitPoint:-:");
            string url = "https://localhost:7045/api/Auth/";

            JCheckUser jCheckUser = new JCheckUser()
            {
                token = splitInfo[0],
                username = splitInfo[1]
            };
            var serializedLogin = JsonConvert.SerializeObject(jCheckUser);
            StringContent httpContent = new StringContent(serializedLogin, Encoding.UTF8, "application/json");

            HttpClient clientTest = new HttpClient();
            try
            {
                var response1 = await clientTest.PostAsync(url + "CheckUser", httpContent);
                //Console.WriteLine(response1.Content.ReadAsStringAsync().Result);
                if (response1.StatusCode == HttpStatusCode.OK)
                {
                    return jCheckUser.username;
                }
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }

        public void StopServer()
        {
            serverLive = false;
            server.Stop();
            Console.WriteLine("Server Successfully shut down.");
        }

        public void InboundPackages(NetworkStream client, MyConnection clientCon)
        {
            string clientIp = ((IPEndPoint)(clientCon.myConnection.Client.RemoteEndPoint)).ToString();
            try
            {
                while (serverLive)
                {
                    byte[] msg = new byte[1024]; 
                    int bytes = client.Read(msg, 0, msg.Length);
                    string dataDecoded = Encoding.UTF8.GetString(msg, 0, bytes);

                    Console.WriteLine(clientCon.name + " (" + clientIp + ") wrote: " + dataDecoded);

                    lock (msgLock)
                    {
                        newMessages.Add(new Message(clientCon, dataDecoded));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(clientCon.name + " (" + clientIp + ") has left the server.");
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
                                byte[] encodedMsg = Encoding.UTF8.GetBytes(outClientMsg + m.myMessage);
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
                            Console.WriteLine("!Clients         \"!Clients\"              Shows all clients connected to the server");
                            Console.WriteLine("!Weather         \"!Weather\"              Shows the current weather");
                            Console.WriteLine("!Msg             \"!Msg Hello World\"      Writes server messages to the clients");
                            Console.WriteLine("!Save            \"!Save World1\"          Creates a save file of the current game state");
                            Console.WriteLine("!Load            \"!Load World1\"          Loads a save file if it exists");
                            Console.WriteLine("!Close           \"!Close\"                Shuts down the server");
                            break;
                        case "!Msg": case "!msg":
                            if (commandText.Length >= 6)
                            {
                                string adminMsg = commandText.Substring(6, commandText.Length);
                                byte[] encodedMsg = Encoding.UTF8.GetBytes(outServerMsg + adminMsg);
                                //allAdminMessages.Add(new AdminMessage(commandText.Substring(6, commandText.Length))); 
                                foreach (MyConnection c in allConnections)
                                {
                                    if (c.myConnection.Connected == true)
                                    {
                                        c.myConnection.GetStream().Write(encodedMsg, 0, encodedMsg.Length);
                                    }
                                }
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
                                if (allConnections[i].myConnection.Connected)
                                {
                                    Console.WriteLine(i + 1 + "                 " + ((IPEndPoint)(allConnections[i].myConnection.Client.RemoteEndPoint)).ToString() + "        " + allConnections[i].name);
                                }
                            }
                            break;
                        case "!Weather": case "!weather":

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
