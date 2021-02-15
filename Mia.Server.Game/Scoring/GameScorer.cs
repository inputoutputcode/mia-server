using System;
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
    }
}
