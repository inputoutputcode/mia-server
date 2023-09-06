using System.Collections.Generic;

using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Scoring.Interface
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
