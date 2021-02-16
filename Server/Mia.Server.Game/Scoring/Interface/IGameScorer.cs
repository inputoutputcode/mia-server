using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Scoring.Interface
{
    public interface IGameScorer
    {
        bool IsGameOver { get; }

        void AddScore(IPlayerList playerList, IPlayer winner, IPlayer looser);

        string GetScoreList(IPlayerList playerList);
    }
}
