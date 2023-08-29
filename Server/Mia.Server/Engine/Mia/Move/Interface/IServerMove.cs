using System;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;


namespace Game.Server.Engine.Mia.Move.Interface
{
    public interface IServerMove
    {
        ServerMoveCode Code { get; }

        ServerFailureReasonCode FailureReasonCode { get; }

        string Value { get; }

        Guid Token { get; }

        IPlayer[] Players { get; }
    }
}
