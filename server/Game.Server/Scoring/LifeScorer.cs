using Game.Server.Engine.Mia.Interface;

namespace Game.Server.Scoring
{
    internal class LifeScorer : GameScorerBase
    {
        public override void Neutral(IPlayer player)
        {
            player.Score -= 1;
        }

        public override void Lost(IPlayer player)
        {
        }

        public override void Winner(IPlayer player)
        {
        }
    }
}