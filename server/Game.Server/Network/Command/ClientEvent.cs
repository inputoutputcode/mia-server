using Game.Server.Network.Command.Interface;

using LiteNetLib;


namespace Game.Server.Network.Command
{
    public class ClientEvent : IClientEvent
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

        public ClientEvent()
        {
        }

        public ClientEvent(string message, NetPeer peer)
        {
            this.message = message;
            this.peer = peer;
        }
    }
}
