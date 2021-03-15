using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Scoring
{
    internal class LifeScorer : GameScorerBase
    {
        public override void Looser(IPlayer player)
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