using System;

using Game.Mia.Bot.Nightmare.Game;
using Game.Mia.Bot.Nightmare.Network;


namespace Game.Mia.Bot.Nightmare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Config.Config.Settings.PlayerName;

            var client = new Client(new ClientListener(new GameLogic()), true);
            client.Start();

            Console.ReadKey();
        }
    }
}
