using LiteNetLib;


namespace Mia.Server.Game.Communication.Command.Interface
{
    public interface ICommand
    {
        string CommandText { get; }

        NetPeer Peer { get; }
    }
}
