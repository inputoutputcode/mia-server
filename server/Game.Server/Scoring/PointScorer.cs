using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Scoring
{
    internal class PointScorer : GameScorerBase
    {
        public override void Neutral(IPlayer player)
        {
        }

        public override void Lost(IPlayer player)
        {
            var scoredPlayer = FindPlayer(player.Name, player.IPAddress);
            if (scoredPlayer != null && scoredPlayer.CurrentState == PlayerState.Active)
            {
                int scoreDeduction = Config.Config.Settings.ScoreDecrement;
                if ((scoredPlayer.Score - scoreDeduction) > 0)
                    scoredPlayer.Score -= scoreDeduction;
                else
                    scoredPlayer.Score = 0;
            }
                
        }

        public override void Winner(IPlayer player)
        {
            var scoredPlayer = FindPlayer(player.Name, player.IPAddress);
            if (scoredPlayer != null && scoredPlayer.CurrentState == PlayerState.Active)
                scoredPlayer.Score += Config.Config.Settings.ScoreIncrement;
        }
    }
}