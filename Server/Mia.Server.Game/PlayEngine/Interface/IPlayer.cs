using Mia.Server.Game.PlayEngine;


namespace Mia.Server.Game.Interface
{
    public interface IPlayer
    {
        bool IsSpectator { get; set; }

        string Name { get; }

        PlayerState CurrentState { get; }

        int Score { get; }
    }
}
