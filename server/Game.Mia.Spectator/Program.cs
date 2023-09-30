using Game.Server.Spectator.Game;
using Game.Server.Spectator.Network;


namespace Game.Server.Spectator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Configuration.Config.Settings.PlayerName;

            var client = new Network.Client(new ClientListener(new GameLogic()), true);
            client.Start();

            Console.ReadKey();
        }
    }
}