using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Mia.Server.Game.Register;


namespace Mia.Server.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mia Server";

            using (var consoleCopy = new ConsoleCopy(string.Format(@"C:\Temp\Maxle_{0}.log", DateTime.Now.Ticks)))
            {
                var server = new GameManager(Config.Settings.ServerPort);

                Console.WriteLine($"Mia serer is listening on {LocalIPAddress}:{Config.Settings.ServerPort}");
            }

            Console.WriteLine("Game over");
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
