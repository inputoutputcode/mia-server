using System;

using Mia.Server.Messaging;
using Mia.Server;
using Mia.Server.Logging;


namespace Sample.Maxle.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mia Server";

            using (var consoleCopy = new ConsoleCopy(string.Format(@"C:\Temp\Maxle_{0}.log", DateTime.Now.Ticks)))
            {
                var server = new MiaGameManager(MessageServer.Instance);
            }

            Console.WriteLine("Game over");
            Console.ReadKey();
        }
    }
}
