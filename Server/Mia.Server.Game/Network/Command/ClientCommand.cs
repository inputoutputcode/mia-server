using Mia.Server.Game.Network.Command.Interface;

using LiteNetLib;


namespace Mia.Server.Game.Network.Command
{
    public class ClientCommand : IClientCommand
    {
        private string command;
        private NetPeer peer;

        public string CommandText
        {
            get { return command; }
        }

        public NetPeer Peer
        {
            get { return peer; }
        }

        public ClientCommand()
        {
        }

        public ClientCommand(string command, NetPeer peer)
        {
            this.command = command;
            this.peer = peer;
        }
    }
}
