using System;

using Mia.Server.ConsoleRunner.Config;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Scoring
{
    internal class MoneyScorer : GameScorerBase
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