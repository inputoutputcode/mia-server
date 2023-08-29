using System;
using System.Collections.Generic;

using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Network.Command.Interface;
using Game.Server.Network.Interface;

using LiteNetLib;


namespace Game.Server.Register.Interface
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
