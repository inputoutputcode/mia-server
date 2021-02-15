using System;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move.Interface
{
    public interface IServerTurn
    {
        ServerMoveCode Code { get; }

        string Value { get; }

        Guid GameToken { get; }

        IPlayer Player { get; }
    }
}
