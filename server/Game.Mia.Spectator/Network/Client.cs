using System.Net;
using System.Net.Sockets;

using Game.Server.Spectator.Configuration;

using LiteNetLib;


namespace Game.Server.Spectator.Network
{
    public class Client
    {
        private bool isLocalServer;
        private ClientListener listener;

        public Client(ClientListener listener, bool isLocalServer = false)
        {
            this.isLocalServer = isLocalServer;
            this.listener = listener;
        }

        public void Start()
        { 
            string serverAddress = Config.Settings.ServerAddress;

            if (isLocalServer)
                serverAddress = LocalIPAddress.ToString();

            Thread.Sleep(500);

            var client = new NetManager(listener);
            client.AutoRecycle = true;
            client.DisconnectTimeout = 60000;
            client.Start(Config.Settings.LocalPort);
            client.Connect(serverAddress, Config.Settings.ServerPort, Config.Settings.ConnectionKey);

            while (!Console.KeyAvailable)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }

            client.Stop();
        }

        public IPAddress LocalIPAddress
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
