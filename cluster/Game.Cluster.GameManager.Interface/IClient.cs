namespace Game.Cluster.GameManager.Interface
{
    public interface IClient
    {
        string PlayerName { get; set; }

        string IPAddressPort { get; set; }
    }
}
