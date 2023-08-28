using System.Net;
using LiteNetLib;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Command
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
