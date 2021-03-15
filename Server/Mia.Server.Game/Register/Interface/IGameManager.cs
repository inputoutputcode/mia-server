using System;
using System.Collections.Generic;
using System.Net;
using Mia.Server.Game.Communication.Command.Interface;
using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.PlayEngine.Move.Interface;


namespace Mia.Server.Game.Register.Interface
{
    public interface IGameManager
    {
        List<IGameInstance> ActiveGames { get; }

        void ProcessCommand(IClientCommand command);

        IClient FindClient(IPEndPoint endPoint);

        IClient FindClient(string playerName);

        IGameInstance FindGameInstance(Guid gameToken);

        IGameInstance FindGameInstance(string name);

        IGame FindGame(Guid gameToken);

        bool ValidateName(string playerName);

        void ProcessMove(IServerMove serverMove);
    }
}
