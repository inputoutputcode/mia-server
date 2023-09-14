using System;


namespace Game.Server.Register.Interface
{
    public interface IGameInstance
    {
        Guid GameToken { get; }

        string Name { get; }
    }
}
