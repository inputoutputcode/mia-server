using System.Net;
using LiteNetLib;
using Game.Server.Network.Command.Interface;


namespace Game.Server.Network.Command
{
    public class ServerEvent : IServerEvent
    {
        private string message;
        private NetPeer peer;

        public string Message
        {
            get { return message; }
        }

        public NetPeer Peer
        {
            get { return peer; }
        }

        public ServerEvent()
        {
        }

        public ServerEvent(string message, NetPeer peer)
        {
            this.message = message;
            this.peer = peer;
        }
    }
}
