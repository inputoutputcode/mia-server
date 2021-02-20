using System.Net;
using System.Net.Sockets;


namespace Mia.Server.Bot.Starter
{
    public struct UdpState
    {
        public UdpClient UdpClient;
        public IPEndPoint EndPoint;
    }
}
