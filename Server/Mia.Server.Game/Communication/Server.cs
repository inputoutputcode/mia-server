using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using Mia.Server.Game.Communication.Command;
using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Monitoring;
using Mia.Server.Game.Register.Interface;

namespace Mia.Server.Game.Communication
{
    public class Server : IServer
    {
        private readonly IGameManager gameManager;
        private int port;
        private NetManager server;

        public Server(IGameManager gameManager)
        {
            this.gameManager = gameManager;
        }    
        
        public async void CreateServer(int port)
        {
            this.port = port;

            Log.Write($"Server starting on port {port}");

            var listener = new ServerListener(gameManager);
            server = new NetManager(listener);
            server.AutoRecycle = true;
            server.DisconnectTimeout = 60000;
            listener.Server = server;
            server.Start(port);

            await RunServerInfiniteAsync();
        }

        private async Task RunServerInfiniteAsync()
        {
            await Task.Run(() =>
            {
                while (!Console.KeyAvailable)
                {
                    server.PollEvents();

                    //Log.Write(string.Format("ServStats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                    //    server.Statistics.BytesReceived,
                    //    server.Statistics.PacketsReceived,
                    //    server.Statistics.BytesSent,
                    //    server.Statistics.PacketsSent));

                    Thread.Sleep(15);
                }

                server.Stop();
            });
        }

        public void SendToAll(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            server.SendToAll(messageBytes, DeliveryMethod.ReliableOrdered);
        }
    }
}
