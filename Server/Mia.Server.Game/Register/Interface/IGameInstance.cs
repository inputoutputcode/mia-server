using System;


namespace Mia.Server.Game.Register.Interface
{
    public interface IGameInstance
    {
        Guid GameToken { get; }

        int GameNumber { get; }
    }
}
