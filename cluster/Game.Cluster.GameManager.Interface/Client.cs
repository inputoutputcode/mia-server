using System.Runtime.Serialization;


namespace Game.Cluster.GameManager.Interface
{
    [DataContract]
    [KnownType(typeof(Game.Cluster.GameManager.Interface.Client))]
    public class Client : IClient
    {
        [DataMember(Name = "pn")]
        public string PlayerName { get; set; }

        [DataMember(Name = "ip")]
        public string IPAddressPort { get; set; }
    }
}
