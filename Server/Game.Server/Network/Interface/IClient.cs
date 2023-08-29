using LiteNetLib;


namespace Game.Server.Network.Interface
{
    public interface IClient
    {
        string Name { get; }

        NetPeer Peer { get; }
    }
}
