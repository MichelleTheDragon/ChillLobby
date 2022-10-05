using System.Threading;

namespace ChillLobbyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TCP

            ServerCommands serverCommands = new ServerCommands();
            serverCommands.StartServer();

            Thread InboundThread = new Thread(new ThreadStart(serverCommands.InboundPackages));
            Thread OutboundThread = new Thread(new ThreadStart(serverCommands.OutboundPackages));
            Thread ConsoleCommands = new Thread(new ThreadStart(serverCommands.ConsoleCommands));

            InboundThread.Start();
            OutboundThread.Start();
            ConsoleCommands.Start();
        }
    }
}