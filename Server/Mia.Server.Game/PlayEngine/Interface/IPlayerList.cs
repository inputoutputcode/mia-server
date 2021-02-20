using System.Collections.Generic;


namespace Mia.Server.Game.Interface
{
    public interface IPlayerList
    {
        int MaximumActivePlayers { get; }

        int MaximumSpectators { get; }

        List<IPlayer> RegisteredPlayers { get; }

        List<IPlayer> Spectators { get; }

        List<IPlayer> ActivePlayers { get; }

        void PermutePlayers();

        IPlayer FirstPlayer();

        IPlayer NextPlayer();

        IPlayer PreviousPlayer();

        bool JoinGame(IPlayer player);

        IPlayer FindPlayer(string name);

        void RoundReset();
    }
}
