using Game.Server.Network.Command.Interface;

using LiteNetLib;


namespace Game.Server.Network.Command
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
