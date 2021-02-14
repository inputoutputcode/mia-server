using System.Net;
using System.Net.Sockets;


namespace Mia.Bot.Winner
{
    public struct UdpState
    {
        public UdpClient UdpClient;
        public IPEndPoint EndPoint;
    }
}
