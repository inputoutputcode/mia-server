using System;
using System.Collections.Generic;

using Game.Server.Network.Interface;


namespace Game.Server.Network
{
    /// <summary>
    /// The class controls a registered list of players and spectators. 
    /// </summary>
    internal class ClientList : IClientList
    {
        #region Members

        private List<IClient> clientList;


        #endregion Members


        #region Properties

        #endregion Properties


        #region Constructor

        public ClientList()
        {
            clientList = new List<IClient>();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Add a new player.
        /// </summary>
        /// <param name="player"></param>
        private void Add(IClient client)
        {
            if (clientList.Find(p => p.Name == client.Name) == null)
                clientList.Add(client);
        }

        public bool Register(IClient client)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
