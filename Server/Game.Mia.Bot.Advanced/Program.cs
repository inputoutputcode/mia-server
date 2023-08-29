using System;

using Game.Mia.Bot.Advanced.Game;
using Game.Mia.Bot.Advanced.Network;


namespace Game.Mia.Bot.Advanced
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
