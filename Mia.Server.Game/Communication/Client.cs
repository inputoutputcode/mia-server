using System.Net;
using Mia.Server.Game.Communication.Interface;


namespace Mia.Server.Game.Communication
{
    internal class Client : IClient
    {
        #region Members

        private string name;
        private IPEndPoint endPoint;

        #endregion Members


        #region Properties

        public string Name
        {
            get { return name; }
        }

        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        public int Score { get; set; }

        #endregion Properties


        #region Constructor

        public Client(string name, IPEndPoint endPoint)
        {
            this.name = name;
            this.endPoint = endPoint;
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
