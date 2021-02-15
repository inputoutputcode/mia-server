using System.Net;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Command
{
    public class ServerCommand : BaseCommand, IServerCommand
    {
        private string command;
        private IPEndPoint endPoint;

        public string Command
        {
            get { return command; }
        }

        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        public ServerCommand()
        {
        }

        public ServerCommand(string command, IPEndPoint endPoint)
        {
            this.command = command;
            this.endPoint = endPoint;
        }
    }
}
