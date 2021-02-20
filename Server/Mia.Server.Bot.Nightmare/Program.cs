using System;


namespace Mia.Server.Bot.Nightmare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Configuration.Config.Settings.PlayerName;
            new GamePlay(true);
            Console.ReadKey();
        }
    }
}
