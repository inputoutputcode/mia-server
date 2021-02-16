using System;
using System.Collections.Generic;
using System.Linq;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine
{
    /// <summary>
    /// The class controls a registered list of players and spectators. 
    /// </summary>
    internal class PlayerList : IPlayerList
    {
        #region Members

        private List<IPlayer> players;
        private int currentPlayerIndex = 0;

        #endregion Members


        #region Properties

        public int PlayerCount
        {
            get
            {
                return ActivePlayers.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return PlayerCount == 0;
            }
        }

        public bool HasPlayer
        {
            get
            {
                return ActivePlayers.Count > 0;
            }
        }

        /// <summary>
        /// Get all players without spectators.
        /// </summary>
        public List<IPlayer> ActivePlayers
        {
            get
            {
                return players.FindAll(p => !p.IsSpectator);
            }
        }

        /// <summary>
        /// Get all players with the spectators.
        /// </summary>
        public List<IPlayer> RegisteredPlayers
        {
            get
            {
                return players;
            }
        }

        #endregion Properties


        #region Constructor

        public PlayerList()
        {
            players = new List<IPlayer>();
        }

        public PlayerList(List<IPlayer> registeredPlayers)
        {
            players = registeredPlayers;
        }

        #endregion


        #region Methods

        public bool AddPlayer(IPlayer player)
        {
            bool isNewPlayer = players.Exists(x => x.Name == player.Name);
            if (isNewPlayer)
            {
                players.Add(player);
            }

            return isNewPlayer;
        }

        public IPlayer FindPlayer(string name)
        {
            return players.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Shake the order in player list.
        /// </summary>
        public void PermutePlayers()
        { 
            players = players.OrderBy(p => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Add a new player.
        /// </summary>
        /// <param name="player"></param>
        public void Add(IPlayer player)
        {
            if (players.Find(p => p.Name == player.Name) == null)
                players.Add(player);
        }

        /// <summary>
        /// Get the first player. Reset the current player index.
        /// </summary>
        /// <returns>Return null if Size = 0.</returns>
        public IPlayer FirstPlayer()
        {
            if (HasPlayer)
            {
                currentPlayerIndex = 0;
                return ActivePlayers[0];
            }

            return null;
        }

        public IPlayer PreviousPlayer()
        {
            int lastPlayerIndex;

            if (currentPlayerIndex >= 0 && currentPlayerIndex < PlayerCount - 1)
            {
                lastPlayerIndex = currentPlayerIndex - 1;
            }
            else
            {
                lastPlayerIndex = PlayerCount - 1;
            }

            return players[lastPlayerIndex];
        }

        /// <summary>
        /// Get a tuple with players in the order: current player, last player.
        /// </summary>
        /// <returns></returns>
        public IPlayer NextPlayer()
        {
            if (currentPlayerIndex >= 0 && currentPlayerIndex < PlayerCount - 1)
            {
                currentPlayerIndex++;
            }
            else
            {
                currentPlayerIndex = 0;
            }

            return players[currentPlayerIndex];
        }

        #endregion Methods
    }
}
