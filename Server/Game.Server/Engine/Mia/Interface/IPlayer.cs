namespace Game.Server.Engine.Mia.Interface
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
