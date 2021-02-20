using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Mia.Server.Game.Communication.Command;
using Mia.Server.Game.Communication.Command.Interface;
using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Monitoring;


namespace Mia.Server.Game.Communication
{
    public class CommandServer : ICommandServer
    {
        private UdpClient udpClient;

        /// <summary>
        /// This queue collects all incoming messages
        /// </summary>
        private ConcurrentQueue<Tuple<string, IPEndPoint>> messageQueue = new ConcurrentQueue<Tuple<string, IPEndPoint>>();

        public CommandServer(int port)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            udpClient = new UdpClient(localEndPoint);

            UdpState udpStateServer = new UdpState
            {
                EndPoint = localEndPoint,
                UdpClient = udpClient
            };

            udpStateServer.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpStateServer);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var udpClient = ((UdpState)(result.AsyncState)).UdpClient;
            var endPoint = ((UdpState)(result.AsyncState)).EndPoint;

            byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);

            UdpState udpStateClient = new UdpState
            {
                EndPoint = endPoint,
                UdpClient = udpClient
            };
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpStateClient);

            string receivedValue = Encoding.UTF8.GetString(receivedBytes);
            ReceiveCommand(receivedValue, endPoint);
        }

        public void ReceiveCommand(string receivedValue, IPEndPoint endPoint)
        {
            try
            {
                messageQueue.Enqueue(new Tuple<string, IPEndPoint>(receivedValue, endPoint));
            }
            catch (Exception ex)
            {
                Log.Message(ex.Message);
            }
        }

        public IClientCommand GetClientCommand()
        {
            Tuple<string, IPEndPoint> clientMessage;
            bool isDequeued = messageQueue.TryDequeue(out clientMessage);

            if (isDequeued)
                return new ClientCommand(clientMessage.Item1, clientMessage.Item2);

            return null;
        }

        public void SendCommand(IServerCommand serverCommand)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(serverCommand.CommandText);

            udpClient.Send(messageBytes, messageBytes.Length, serverCommand.EndPoint);
        }
    }
}
