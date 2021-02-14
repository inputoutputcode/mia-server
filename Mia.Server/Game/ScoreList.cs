using System;
using System.Collections.Generic;


namespace Mia.Server
{
    public class ScoreList
    {
        #region Members

        private int gameCount;
        private PlayerList playerScores;

        #endregion Members


        #region Properties

        public PlayerList PlayerScores
        {
            get { return playerScores; }
        }

        public int GameCount
        {
            get { return gameCount; }
        }

        #endregion Properties


        #region Constructor

        public ScoreList(PlayerList players)
        {
            playerScores = players;
        }

        #endregion Constructor


        #region Methods

        public void AddGameCount()
        {
            gameCount++;
        }

        public int ScoreOf(Player player)
        {
            var playerScore = PlayerScores.RealPlayers.Find(p => p.Name == player.Name);

            if (playerScore != null)
                return playerScore.Score;

            return 0;
        }

        public void IncreaseFor(Player player)
        {
            var playerScore = PlayerScores.RealPlayers.Find(p => p.Name == player.Name);

            if (playerScore != null)
                playerScore.Score++;
        }

        public void DecreaseFor(Player player)
        {
            var playerScore = PlayerScores.RealPlayers.Find(p => p.Name == player.Name);

            if (playerScore != null)
                playerScore.Score--;
        }

        public string AllScores()
        {
            string scores = string.Empty;

            foreach (Player player in PlayerScores.RealPlayers)
            { 
                scores += player.Name + ":" + player.Score + ",";          
            }

            scores = scores.Remove(scores.Length - 1);

            return scores;
        }

        #endregion Methods
    }
}
