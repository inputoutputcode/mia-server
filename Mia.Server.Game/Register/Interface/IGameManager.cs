using System;
using System.Collections.Generic;
using System.Net;
using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.Register.Interface
{
    public interface IGameManager
    {
        List<IGameInstance> ActiveGames { get; }

        IGame FindGame(Guid gameToken);

        IClient FindClient(IPEndPoint endPoint);

        IClient FindClient(string playerName);

        
    }
}
