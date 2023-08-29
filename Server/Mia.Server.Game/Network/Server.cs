using System;
using System.Threading;
using System.Threading.Tasks;

using Mia.Server.Game.Network.Interface;
using Mia.Server.Game.Logging;
using Mia.Server.Game.Register.Interface;

using LiteNetLib;


namespace Mia.Server.Game.Network
{
    public class Server : IServer
    {
        private readonly IGameManager gameManager;
        private NetManager server;

        public Server(IGameManager gameManager)
        {
            this.gameManager = gameManager;
        }    
        
        public async void CreateServer(int port)
        {
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
    }
}
