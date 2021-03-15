using System.Collections.Generic;

using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring.Interface;


namespace Mia.Server.Game.Scoring
{
    public abstract class GameScorerBase : IGameScorer
    {
        private List<IPlayer> playerList;

        public void SetActivePlayers(List<IPlayer> playerList)
        {
            this.playerList = playerList;
        }

        public abstract void Lost(IPlayer player);

        public abstract void Looser(IPlayer player);

        public abstract void Winner(IPlayer player);

        public List<IPlayer> GetScores()
        {
            return new List<IPlayer>(playerList);
        }

        public string GetScoreValues()
        {
            string scores = string.Empty;

            foreach (IPlayer player in playerList)
            {
                scores += player.Name + ":" + player.Score + ",";
            }

            scores = scores.Remove(scores.Length - 1);

            return scores;
        }
    }
}
