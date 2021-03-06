﻿using System;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move.Interface
{
    public interface IServerMove
    {
        ServerMoveCode Code { get; }

        ServerFailureReasonCode FailureReasonCode { get; }

        string Value { get; }

        Guid Token { get; }

        IPlayer[] Players { get; }
    }
}
