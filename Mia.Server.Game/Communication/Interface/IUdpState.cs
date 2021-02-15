using System.Net;
using System.Net.Sockets;


namespace Mia.Server.Game.Communication.Interface
{
    public interface IUdpState
    {
        UdpClient UdpClient { get; set; }
        IPEndPoint EndPoint { get; set; }
    }
}
