using System;
using System.Collections.Generic;

using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Network.Event.Interface;
using Game.Server.Network.Interface;

using LiteNetLib;


namespace Game.Server.Register.Interface
{
    public interface IGameManager
    {
        List<IGameInstance> ActiveGamesInstances { get; }

        void ReceiveEvent(IClientEvent clientEvent);

        void ReceiveEventMessage(string eventMessage, NetPeer peer);

        IClient FindClient(NetPeer peer);

        IClient FindClient(string playerName);

        IGameInstance FindGameInstance(Guid gameToken);

        IGameInstance FindGameInstance(string name);

        IGame FindGame(Guid gameToken);

        bool ValidateName(string playerName);

        void SendEvent(string eventMessage, IPlayer[] players);
    }
}
