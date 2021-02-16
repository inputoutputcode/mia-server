using System;
using System.Collections.Generic;
using System.Net;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Interface
{
    public interface ICommandServer
    {
        IClientCommand GetClientCommand();

        void SendCommand(IServerCommand command);

        void SendCommand(IServerCommand command, List<IPEndPoint> endPoints);

        void ReceiveCallback(IAsyncResult result);
    }
}
