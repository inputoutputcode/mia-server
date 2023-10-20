using Game.Cluster.GameManager.Interface;


namespace Game.Cluster.Gateway.Bla
{
    public class Client : IClient
    {
        public string PlayerName { get; set; }

        public string IPAddressPort { get; set; }
    }
}
