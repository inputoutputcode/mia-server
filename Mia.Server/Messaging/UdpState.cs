using System;
using System.Net;
using System.Net.Sockets;


namespace Mia.Server.Messaging
{
    public class UdpState
    {
        public UdpClient UdpClient;
        public IPEndPoint EndPoint;
    }
}
