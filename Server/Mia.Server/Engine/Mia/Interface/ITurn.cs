namespace Game.Server.Engine.Mia.Interface
{
    public interface ITurn
    {
        IPlayer Player { get; }

        int RollCount { get; }

        void AddRollCount();
    }
}
