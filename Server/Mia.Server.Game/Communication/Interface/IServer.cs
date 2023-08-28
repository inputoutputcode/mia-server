using System;
using System.Net;
using LiteNetLib;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Interface
{
    public interface IServer
    {
        void CreateServer(int port);

        void SendToAll(string message);

        //event EventHandler ProcessEvent;

        //event EventHandler SendEvent;

        //void ProcessEvent(string eventMessage, NetPeer peer);

        //void SendEvent(IServerCommand serverCommand);
    }
}
