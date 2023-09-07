using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Game.Server.Engine.Mia.Move.Interface;


namespace Game.Server.Engine.Mia.Interface
{
    public interface IGame
    {
        int TurnCount { get; }

        List<IPlayer> Players { get; }

        Guid Token { get; }

        int GameNumber { get; }

        void CreateSimulation(IPlayerList players, ITurn turn, IDice dice);

        bool Register(IPlayer player);

        Task StartAsync();

        void RoundStarted();

        void ReceiveClientEvent(string eventMessage, string eventValue, IPlayer player, Guid token);

        void SendServerMessage(IServerMove serverMove);

        List<IPlayer> GetScore();
    }
}
