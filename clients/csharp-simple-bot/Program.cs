using System;


namespace Mia.Bot.Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Starter Bot";
            var bot = new StarterBot();
            Console.ReadKey();
        }
    }
}
