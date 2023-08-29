using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Game.Server.Register;
using Game.Server.Console.Logging;


namespace Game.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Mia Server started";

            using (var consoleCopy = new ConsoleCopy(string.Format(@"C:\Temp\Maxle_{0}.log", DateTime.Now.Ticks)))
            {
                var server = new GameManager() { ServerPort = Config.Config.Settings.ServerPort };
                server.Initialize();
            }

            System.Console.WriteLine("Mia Server stopped");
            System.Console.ReadKey();
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
