using System;


namespace Mia.Bot.Winner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Winner Bot";
            var bot = new WinnerBot();
            Console.ReadKey();
        }
    }
}
