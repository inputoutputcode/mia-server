using System.Net;


namespace Mia.Server.Game.Communication.Interface
{
    public interface IClient
    {
        string Name { get; }

        IPEndPoint EndPoint { get; }
    }
}
