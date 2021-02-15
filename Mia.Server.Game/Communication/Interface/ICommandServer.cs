using System;
using System.Collections.Generic;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Interface
{
    public interface ICommandServer
    {
        IClientCommand GetClientCommand();

        void SendCommand(string message, UdpState udpState);

        void SendCommand(string message, List<UdpState> udpStates);

        void ReceiveCallback(IAsyncResult result);
    }
}
