using System;
using Mia.Server.Game.PlayEngine.Move.Interface;


namespace Mia.Server.Game.Interface
{
    public interface IGame
    {
        bool RoundFinished { get; }
        
        int TurnCount { get; }
        
        bool TurnFinished { get; }
        
        IPlayerList Players { get; }

        ITurn CurrentTurn { get; }

        Guid Token { get; }

        int GameNumber { get; }

        void CreateSimulation(IPlayerList players, ITurn turn, IDice dice);

        bool JoinGame(IPlayer player);

        void NewRound();

        void Start();

        void RoundStarted();

        void Move(IPlayerMove playerMove);
    }
}
