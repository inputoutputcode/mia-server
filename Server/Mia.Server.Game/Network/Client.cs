using LiteNetLib;

using Mia.Server.Game.Network.Interface;


namespace Mia.Server.Game.Network
{
    internal class Client : IClient
    {
        #region Members

        private string name;
        private NetPeer peer;

        #endregion Members


        #region Properties

        public string Name
        {
            get { return name; }
        }

        public NetPeer Peer
        {
            get { return peer; }
        }

        public int Score { get; set; }

        #endregion Properties


        #region Constructor

        public Client(string name, NetPeer peer)
        {
            this.name = name;
            this.peer = peer;
        }

        #endregion Constructor


        #region Methods


        public string GetDebuggerDisplay()
        {
            return this.Name;
        }

        #endregion
    }
}
