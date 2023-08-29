using System;
using System.Collections.Generic;

using Mia.Server.Game.Network.Command.Interface;
using Mia.Server.Game.Network.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.PlayEngine.Move.Interface;

using LiteNetLib;


namespace Mia.Server.Game.Register.Interface
{
    public interface IGameManager
    {
        List<IGameInstance> ActiveGamesInstances { get; }

        void ProcessCommand(IClientCommand command);

        void ProcessEvent(string eventMessage, NetPeer peer);

        IClient FindClient(NetPeer peer);

        IClient FindClient(string playerName);

        IGameInstance FindGameInstance(Guid gameToken);

        IGameInstance FindGameInstance(string name);

        IGame FindGame(Guid gameToken);

        bool ValidateName(string playerName);

        void ProcessMove(IServerMove serverMove);
    }
}
