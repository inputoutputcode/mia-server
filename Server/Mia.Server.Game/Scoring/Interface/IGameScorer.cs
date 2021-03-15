using System.Collections.Generic;

using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Scoring.Interface
{
    public interface IGameScorer
    {
        void SetActivePlayers(List<IPlayer> activePlayers);

        void Lost(IPlayer player);

        void Looser(IPlayer player);

        void Winner(IPlayer player);

        string GetScoreValues();

        List<IPlayer> GetScores();
    }
}
