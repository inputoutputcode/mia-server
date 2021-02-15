using System;
using System.Collections.Generic;
using System.Text;

namespace Mia.Server.Game.Register.Interface
{
    public interface IGameInstance
    {
        Guid GameToken { get; }

        T Type { get; }
    }
}
