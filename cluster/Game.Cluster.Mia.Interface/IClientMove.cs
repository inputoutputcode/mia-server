namespace Game.Cluster.Mia.Interface
{
    public interface IClientMove
    {
        ClientMoveCode Code { get; }

        string Value { get; }

        Guid Token { get; }

        IPlayer Player { get; }
    }
}
