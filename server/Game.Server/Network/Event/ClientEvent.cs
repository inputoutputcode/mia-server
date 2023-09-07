using Game.Server.Network.Event.Interface;

using LiteNetLib;


namespace Game.Server.Network.Event
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
