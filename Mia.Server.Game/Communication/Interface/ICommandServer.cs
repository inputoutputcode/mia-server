using System;
using System.Net;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Interface
{
    public interface ICommandServer
    {
        IClientCommand GetClientCommand();

        void ReceiveCommand(string receivedValue, IPEndPoint endPoint);

        void SendCommand(IServerCommand serverCommand);
    }
}
