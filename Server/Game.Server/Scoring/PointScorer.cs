using Game.Server.Console.Config;
using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Scoring
{
    internal class PointScorer : GameScorerBase
    {
        public override void Looser(IPlayer player)
        {
            player.Score -= Config.Settings.ScoreDecrement;
        }

        public override void Lost(IPlayer player)
        {
            player.Score -= Config.Settings.ScoreDecrement / 2;
        }

        public override void Winner(IPlayer player)
        {
            player.Score += Config.Settings.ScoreIncrement;
        }
    }
}