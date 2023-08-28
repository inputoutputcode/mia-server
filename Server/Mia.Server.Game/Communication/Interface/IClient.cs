using LiteNetLib;


namespace Mia.Server.Game.Communication.Interface
{
    public interface IClient
    {
        string Name { get; }

        NetPeer Peer { get; }
    }
}
