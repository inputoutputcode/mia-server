using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Scoring.Interface
{
    public interface IGameScorer
    {
        void AddScore(IPlayerList playerList, IPlayer winner, IPlayer looser);
    }
}
