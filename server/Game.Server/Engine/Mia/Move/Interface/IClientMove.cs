using System;

using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Engine.Mia.Move.Interface
{
    public interface IClientMove
    {
        ClientMoveCode Code { get; }

        string Value { get; }

        Guid Token { get; }

        IPlayer Player { get; }

    }
}