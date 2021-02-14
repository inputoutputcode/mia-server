using System;
using System.Collections.Generic;
using System.Linq;


namespace Mia.Server
{
    /// <summary>
    /// The class controls a registered list of players and spectators. 
    /// </summary>
    public class PlayerList
    {
        #region Members

        private List<Player> players;
        private int currentPlayerIndex = 0;

        #endregion Members


        #region Properties

        public int Size
        {
            get
            {
                return RealPlayers.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Size == 0;
            }
        }

        public bool HasPlayer
        {
            get
            {
                return RealPlayers.Count > 0;
            }
        }

        #endregion Properties


        #region Constructor

        public PlayerList()
        { 
            players = new List<Player>();
        }

        public PlayerList(List<Player> registeredPlayers)
        {
            players = registeredPlayers;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Get all players without spectators.
        /// </summary>
        public List<Player> RealPlayers
        {
            get
            {
                return players.FindAll(p => !p.IsSpectator);
            }
        }

        /// <summary>
        /// Get all players with the spectators.
        /// </summary>
        public List<Player> AllPlayers
        {
            get
            {
                return players;
            }
        }

        /// <summary>
        /// Shake the order in player list.
        /// </summary>
        public void Permute()
        { 
            players = players.OrderBy(p => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Add a new player.
        /// </summary>
        /// <param name="player"></param>
        public void Add(Player player)
        {
            if (players.Find(p => p.Name == player.Name) == null)
                players.Add(player);
        }

        /// <summary>
        /// Get the first player. Reset the current player index.
        /// </summary>
        /// <returns>Return null if Size = 0.</returns>
        public Player FirstPlayer()
        {
            if (HasPlayer)
            {
                currentPlayerIndex = 0;
                return RealPlayers[0];
            }

            return null;
        }

        /// <summary>
        /// Get a tuple with players in the order: current player, last player.
        /// </summary>
        /// <returns></returns>
        public CurrentPlayers NextPlayer()
        {
            int lastPlayerIndex = -1;

            if (currentPlayerIndex > 0 && currentPlayerIndex < Size - 1)
            {
                currentPlayerIndex++;
                lastPlayerIndex = currentPlayerIndex - 1;
            }
            else
            {
                currentPlayerIndex = 0;
                lastPlayerIndex = Size - 1;
            }

            return new CurrentPlayers() { Current = players[currentPlayerIndex], Last = players[lastPlayerIndex] };
        }

        #endregion Methods
    }
}
