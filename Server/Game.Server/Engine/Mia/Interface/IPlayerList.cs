using System.Collections.Generic;


namespace Game.Server.Engine.Mia.Interface
{
    public interface IPlayerList
    {
        int MaximumActivePlayers { get; }

        int MaximumSpectators { get; }

        List<IPlayer> RegisteredPlayers { get; }

        List<IPlayer> Spectators { get; }

        List<IPlayer> ActivePlayers { get; }

        bool Register(IPlayer player);

        bool Join(IPlayer player);

        IPlayer Find(string name);

        IPlayer First();

        IPlayer Current();

        IPlayer Next();

        IPlayer Previous();

        void Permute();

        void RoundReset();
    }
}
