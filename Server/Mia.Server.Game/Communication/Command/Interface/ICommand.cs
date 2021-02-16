using System.Net;


namespace Mia.Server.Game.Communication.Command.Interface
{
    public interface ICommand
    {
        string CommandText { get; }

        IPEndPoint EndPoint { get; }
    }
}
