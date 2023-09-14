using System.Collections.Generic;

using Game.Server.Engine.Mia.Interface;
using Game.Server.Scoring.Interface;


namespace Game.Server.Scoring
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
