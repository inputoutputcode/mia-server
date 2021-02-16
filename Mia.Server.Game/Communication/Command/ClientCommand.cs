using System.Net;
using Mia.Server.Game.Communication.Command.Interface;


namespace Mia.Server.Game.Communication.Command
{
    public class ClientCommand : BaseCommand, IClientCommand
    {
        private string command;
        private IPEndPoint endPoint;

        public string CommandText
        {
            get { return command; }
        }

        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        public ClientCommand()
        {
        }

        public ClientCommand(string command, IPEndPoint endPoint)
        {
            this.command = command;
            this.endPoint = endPoint;
        }
    }
}
