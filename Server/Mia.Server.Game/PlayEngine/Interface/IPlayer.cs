using Mia.Server.Game.PlayEngine;


namespace Mia.Server.Game.Interface
{
    public interface IPlayer
    {
        string Name { get; }

        PlayerState CurrentState { get; }

        int Score { get; set; }

        void SetActive();

        void Kick();
    }
}
