using System.Net;
using LiteNetLib;
using Mia.Server.Game.Network.Command.Interface;


namespace Mia.Server.Game.Network.Command
{
    public class ServerCommand : IServerCommand
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

        public ServerCommand()
        {
        }

        public ServerCommand(string command, NetPeer peer)
        {
            this.command = command;
            this.peer = peer;
        }
    }
}
