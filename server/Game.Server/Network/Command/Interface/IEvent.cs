using LiteNetLib;


namespace Game.Server.Network.Command.Interface
{
    public interface IEvent
    {
        string Message { get; }

        NetPeer Peer { get; }
    }
}
