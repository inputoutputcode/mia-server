using System;
using System.Collections.Generic;
using System.Linq;

using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Engine.Mia
{
    /// <summary>
    /// The class controls a registered list of players and spectators. 
    /// </summary>
    public class PlayerList : IPlayerList
    {
        #region Members

        private List<IPlayer> players;
        private int currentPlayerIndex;
        private int maximumActivePlayers;
        private int maximumSpectators;

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
                return players.FindAll(p => p.CurrentState == PlayerState.Active);
            }
        }

        public List<IPlayer> Spectators
        {
            get
            {
                return players.FindAll(p => p.CurrentState == PlayerState.Spectator);
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

        public int MaximumActivePlayers
        {
            get
            {
                return maximumActivePlayers;
            }
        }

        public int MaximumSpectators
        {
            get
            {
                return maximumSpectators;
            }
        }

        #endregion Properties


        #region Constructor

        public PlayerList(int maximumActivePlayers = 20, int maximumSpectators = 20)
        {
            Initialize(maximumActivePlayers, maximumSpectators);
        }

        #endregion


        #region Methods

        private void Initialize(int maximumActivePlayers, int maximumSpectators)
        {
            players = new List<IPlayer>();
            this.maximumActivePlayers = maximumActivePlayers;
            this.maximumSpectators = maximumSpectators;
        }

        public bool Register(IPlayer player)
        {
            bool isNewPlayer = !players.Exists(x => x.Name == player.Name);
            if (isNewPlayer)
            {
                players.Add(player);
            }

            return isNewPlayer;
        }

        /// <summary>
        /// Player joins the round
        /// </summary>
        /// <param name="player"></param>
        public bool Join(IPlayer player)
        {
            bool operationResult = false;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Name == player.Name && player.CurrentState != PlayerState.Spectator)
                {
                    players[i].SetActive();
                    operationResult = true;
                }
            }

            return operationResult;
        }

        public IPlayer Find(string name)
        {
            return players.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Get the first player. Reset the current player index.
        /// </summary>
        /// <returns>Return null if Size = 0.</returns>
        public IPlayer First()
        {
            if (HasPlayer)
            {
                currentPlayerIndex = 0;
                return ActivePlayers[0];
            }

            return null;
        }

        public IPlayer Current()
        {
            return players[currentPlayerIndex];
        }

        public IPlayer Previous()
        {
            int lastPlayerIndex;

            if (currentPlayerIndex > 0 && currentPlayerIndex <= PlayerCount - 1)
            {
                lastPlayerIndex = currentPlayerIndex - 1;
            }
            else
            {
                lastPlayerIndex = PlayerCount - 1;
            }

            return ActivePlayers[lastPlayerIndex];
        }

        /// <summary>
        /// Get a tuple with players in the order: current player, last player.
        /// </summary>
        /// <returns></returns>
        public IPlayer Next()
        {
            IPlayer nextPlayer = null;

            if (currentPlayerIndex >= 0 && currentPlayerIndex < ActivePlayers.Count - 1)
            {
                currentPlayerIndex++;
            }
            else
            {
                currentPlayerIndex = 0;
            }

            return ActivePlayers[currentPlayerIndex];
        }

        /// <summary>
        /// Shake the order in player list
        /// </summary>
        public void Permute()
        {
            players = players.OrderBy(p => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Shake the order in player list
        /// </summary>
        public void RoundReset()
        {
            ActivePlayers.ForEach(p => p.Kick());
        }

        #endregion Methods
    }
}
