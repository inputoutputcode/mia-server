using System.Collections.Generic;


namespace Mia.Server.Game.Interface
{
    public interface IPlayerList
    {
        List<IPlayer> RegisteredPlayers { get; }

        List<IPlayer> ActivePlayers { get; }

        void PermutePlayers();

        IPlayer FirstPlayer();

        IPlayer NextPlayer();

        IPlayer PreviousPlayer();
    }
}
