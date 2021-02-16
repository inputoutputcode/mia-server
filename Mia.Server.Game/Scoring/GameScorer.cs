using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring.Interface;


namespace Mia.Server.Game.Scoring
{
    public class GameScorer : IGameScorer
    {
        private ScoreMode scoreMode;

        public void AddScore(IPlayerList playerList, IPlayer winner, IPlayer looser)
        {

        }

        public GameScorer(ScoreMode scoreMode)
        {
            this.scoreMode = scoreMode;
        }

        public string GetScoreList(IPlayerList playerList)
        {
            string scores = string.Empty;

            foreach (IPlayer player in playerList.ActivePlayers)
            {
                scores += player.Name + ":" + player.Score + ",";
            }

            scores = scores.Remove(scores.Length - 1);

            return scores;
        }
    }
}
