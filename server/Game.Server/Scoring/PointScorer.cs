using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Scoring
{
    internal class PointScorer : GameScorerBase
    {
        public override void Looser(IPlayer player)
        {
        }

        public override void Lost(IPlayer player)
        {
            var scoredPlayer = FindPlayer(player.Name, player.IPAddress);
            if (scoredPlayer != null && scoredPlayer.CurrentState == PlayerState.Active)
                scoredPlayer.Score -= Config.Config.Settings.ScoreDecrement;
        }

        public override void Winner(IPlayer player)
        {
            var scoredPlayer = FindPlayer(player.Name, player.IPAddress);
            if (scoredPlayer != null && scoredPlayer.CurrentState == PlayerState.Active)
                scoredPlayer.Score += Config.Config.Settings.ScoreIncrement;
        }
    }
}