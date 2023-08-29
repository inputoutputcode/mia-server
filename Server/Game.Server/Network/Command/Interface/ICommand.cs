using LiteNetLib;


namespace Game.Server.Network.Command.Interface
{
    public interface ICommand
    {
        string CommandText { get; }

        NetPeer Peer { get; }
    }
}
