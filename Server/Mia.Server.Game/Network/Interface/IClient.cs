using LiteNetLib;


namespace Mia.Server.Game.Network.Interface
{
    public interface IClient
    {
        string Name { get; }

        NetPeer Peer { get; }
    }
}
