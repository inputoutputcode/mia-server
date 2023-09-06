using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Scoring
{
    internal class MoneyScorer : GameScorerBase
    {
        public override void Looser(IPlayer player)
        {
        }

        public override void Lost(IPlayer player)
        {
            player.Score -= Config.Config.Settings.ScoreDecrement;
        }

        public override void Winner(IPlayer player)
        {
            player.Score += Config.Config.Settings.ScoreIncrement;
        }
    }
}