using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Mia.Server.Game.Register;
using Mia.Server.ConsoleRunner.Logging;


namespace Mia.Server.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mia Server started";

            using (var consoleCopy = new ConsoleCopy(string.Format(@"C:\Temp\Maxle_{0}.log", DateTime.Now.Ticks)))
            {
                var server = new GameManager() { ServerPort = Config.Config.Settings.ServerPort };
                server.Initialize();
            }

            Console.WriteLine("Mia Server stopped");
            Console.ReadKey();
        }

        public static IPAddress LocalIPAddress
        {
            get
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = (from address in hostEntry.AddressList where address.AddressFamily == AddressFamily.InterNetwork select address.ToString()).FirstOrDefault();

                return IPAddress.Parse(ipAddress);
            }
        }
    }
}
