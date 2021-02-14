using System.Net;
using System.Net.Sockets;


namespace Mia.Bot.Starter
{
    public struct UdpState
    {
        public UdpClient UdpClient;
        public IPEndPoint EndPoint;
    }
}
