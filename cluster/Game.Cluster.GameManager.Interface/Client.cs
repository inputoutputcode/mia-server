using System.Runtime.Serialization;


namespace Game.Cluster.GameManager.Interface
{
    [DataContract]
    public class Client : IClient
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public string IPAddressPort { get; set; }
    }
}
