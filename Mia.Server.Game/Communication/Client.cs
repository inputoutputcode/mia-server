using Mia.Server.Game.Communication.Interface;


namespace Mia.Server.Game.Communication
{
    internal class Client : IClient
    {
        #region Members

        private string name;
        private int clientHash;

        #endregion Members


        #region Properties

        public string Name
        {
            get { return name; }
        }

        public int ClientHash
        {
            get { return clientHash; }
        }

        public int Score { get; set; }

        #endregion Properties


        #region Constructor

        public Client(string name, int clientHash)
        {
            this.name = name;
            this.clientHash = clientHash;
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
