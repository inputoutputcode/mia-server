using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mia.Server.Game.PlayEngine.Move.Interface;


namespace Mia.Server.Game.Interface
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

        void Move(IPlayerMove playerMove);

        List<IPlayer> GetScore();
    }
}
