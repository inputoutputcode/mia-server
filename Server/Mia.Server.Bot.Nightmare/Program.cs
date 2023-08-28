using Mia.Server.Bot.Nightmare.Game;
using Mia.Server.Bot.Nightmare.Network;
using System;


namespace Mia.Server.Bot.Nightmare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Configuration.Config.Settings.PlayerName;

            var client = new Client(new ClientListener(new GameLogic()), true);
            client.Start();

            Console.ReadKey();
        }
    }
}
