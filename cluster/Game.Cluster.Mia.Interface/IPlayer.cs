namespace Game.Cluster.Mia.Interface
{
    public interface IPlayer
    {
        string Name { get; }

        PlayerState CurrentState { get; set; }

        int Score { get; set; }

        string IPAddress { get; }
    }
}
