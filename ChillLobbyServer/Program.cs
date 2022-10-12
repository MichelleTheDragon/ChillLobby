using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;

namespace ChillLobbyServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            ServerCommands serverCommands = new ServerCommands();
            serverCommands.StartServer();
            string serverToken;

            string url = "https://localhost:7045/api/Auth/";

            JServerLogin jServerLogin = new JServerLogin() { 
                username = "IamServer", 
                password = "IamSecret"
            };
            var serializedLogin = JsonConvert.SerializeObject(jServerLogin);
            StringContent httpContent = new StringContent(serializedLogin, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            try
            {
                var response1 = await client.PostAsJsonAsync(url + "RegisterServer", httpContent);

                if (response1.StatusCode == HttpStatusCode.OK)
                {
                    var response2 = await client.PostAsJsonAsync(url + "LoginServer", httpContent);
                    if (response2.StatusCode == HttpStatusCode.OK)
                    {
                        serverToken = response2.Content.ReadAsStringAsync().Result;
                        Thread OutboundThread = new Thread(new ThreadStart(serverCommands.OutboundPackages));
                        Thread UpdateThread = new Thread(new ThreadStart(serverCommands.UpdateAsync));
                        Thread ConsoleCommandsThread = new Thread(new ThreadStart(serverCommands.ConsoleCommands));

                        OutboundThread.Start();
                        UpdateThread.Start();
                        ConsoleCommandsThread.Start();
                    } else
                    {
                        Console.WriteLine(response2.StatusCode.GetHashCode() + ": \"" + response2.StatusCode.ToString() + "\"");
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}