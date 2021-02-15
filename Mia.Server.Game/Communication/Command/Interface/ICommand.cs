using System.Net;


namespace Mia.Server.Game.Communication.Command.Interface
{
    public interface ICommand
    {
        string Command { get; }

        IPEndPoint EndPoint { get; }
    }
}
