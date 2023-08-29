using LiteNetLib;


namespace Mia.Server.Game.Network.Command.Interface
{
    public interface ICommand
    {
        string CommandText { get; }

        NetPeer Peer { get; }
    }
}
