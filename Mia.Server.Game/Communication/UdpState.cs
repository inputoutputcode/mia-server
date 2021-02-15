using System.Net;
using System.Net.Sockets;
using Mia.Server.Game.Communication.Interface;


namespace Mia.Server.Game.Communication
{
    public class UdpState : IUdpState
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;

        public UdpClient UdpClient { get => udpClient; set => udpClient = value; }

        public IPEndPoint EndPoint { get => endPoint; set => endPoint = value; }
    }
}
