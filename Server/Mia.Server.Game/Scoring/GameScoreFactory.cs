using Mia.Server.Game.Scoring.Interface;

namespace Mia.Server.Game.Scoring
{
    public static class GameScoreFactory
    {
        public static IGameScorer Create(ScoreMode mode)
        {
            IGameScorer gameScorer = null;

            switch (mode)
            {
                case ScoreMode.Points:
                    gameScorer = new PointScorer();
                    break;

                case ScoreMode.Lifes:
                    gameScorer = new LifeScorer();
                    break;

                case ScoreMode.Money:
                    gameScorer = new MoneyScorer();
                    break;
            }

            return gameScorer;
        }
    }
}
