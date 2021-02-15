using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Mia.Server.Game.Communication.Command;
using Mia.Server.Game.Communication.Command.Interface;
using Mia.Server.Game.Communication.Interface;


namespace Mia.Server.Game.Communication
{
    public class CommandServer : ICommandServer
    {
        /// <summary>
        /// This queue collects all incoming messages
        /// </summary>
        private ConcurrentQueue<Tuple<string, IPEndPoint>> messageQueue = new ConcurrentQueue<Tuple<string, IPEndPoint>>();

        public CommandServer()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, Config.ServerPort);
            var udpClient = new UdpClient(localEndPoint);

            UdpState udpStateServer = new UdpState
            {
                EndPoint = localEndPoint,
                UdpClient = udpClient
            };

            udpStateServer.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpStateServer);
        }

        public void ReceiveCallback(IAsyncResult result)
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

            try
            {
                string receivedValue = Encoding.UTF8.GetString(receivedBytes);

                messageQueue.Enqueue(new Tuple<string, IPEndPoint>(receivedValue, endPoint));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public IClientCommand GetClientCommand()
        {
            Tuple<string, IPEndPoint> clientMessage;
            bool isDequeued = messageQueue.TryDequeue(out clientMessage);

            if (isDequeued)
                return new ClientCommand(clientMessage.Item1, clientMessage.Item2);

            return new ClientCommand();
        }

        public void SendCommand(string message, UdpState udpState)
        {
            SendCommand(message, new List<UdpState> { udpState });
        }

        /// <summary>
        /// Send a one way message to end points
        /// </summary>
        /// <param name="message"></param>
        /// <param name="endPoints"></param>
        public void SendCommand(string message, List<UdpState> udpStates)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            foreach (UdpState udpState in udpStates)
            {
                udpState.UdpClient.Send(messageBytes, messageBytes.Length, udpState.EndPoint);
            }
        }

    }
}
