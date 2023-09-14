using LiteNetLib;


namespace Game.Server.Network.Event.Interface
{
    public interface IEvent
    {
        string Message { get; }

        NetPeer Peer { get; }
    }
}
