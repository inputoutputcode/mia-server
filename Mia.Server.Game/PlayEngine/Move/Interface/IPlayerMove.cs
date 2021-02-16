using System;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move.Interface
{
    public interface IPlayerMove
    {
        PlayerMoveCode Code { get; }

        string Value { get; }

        Guid Token { get; }

        IPlayer Player { get; }

    }
}