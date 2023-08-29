using System;

using Game.Server.Bot.Starter.Game;
using Game.Server.Bot.Starter.Network;


namespace Game.Server.Bot.Starter
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
