namespace Mia.Server.Game.Interface
{
    public interface ITurn
    {
        IPlayer Player { get; }

        int RollCount { get; }

        bool HasAnnounced { get; }

        bool HasFinished { get; }
    }
}
