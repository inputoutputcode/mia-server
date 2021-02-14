using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;

using Mia.Server.Logging;


namespace Mia.Server.Messaging
{
    /// <summary>
    /// This class handles all connections and message exchange with the UDP protocoll.
    /// </summary>
    public class MessageServer
    {
        #region Members

        private static MessageServer currentInstance;
        private MiaGameServer miaServer;

        #endregion Members


        #region Properties

        public static MessageServer Instance
        { 
            get 
            {
                if (currentInstance == null)
                    currentInstance = new MessageServer();

                return currentInstance;
            }
        }

        public MiaGameServer MiaServerInstance
        {
            set { miaServer = value; }
        }

        public IPAddress LocalIPAddress
        {
            get
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = (from address in hostEntry.AddressList where address.AddressFamily == AddressFamily.InterNetwork select address.ToString()).FirstOrDefault();

                return IPAddress.Parse(ipAddress);
            }
        }

        #endregion Properties


        #region Constructor

        private MessageServer()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, Config.ServerPort);
            var udpClient = new UdpClient(localEndPoint);

            UdpState udpStateServer = new UdpState
            {
                EndPoint = localEndPoint,
                UdpClient = udpClient
            };

            udpStateServer.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpStateServer);
            WaitForRegistration();
        }

        #endregion Constructor


        #region Methods

        public void SendMessageToAll(string message)
        {
            Logger.Log(message);
            
            var udpStates = GetAllPlayerEndPoints();
            SendInfo(message, udpStates);
        }

        private List<UdpState> GetAllPlayerEndPoints()
        {
            var udpStates = new List<UdpState>();

            if (miaServer != null)
            {
                foreach (Player player in miaServer.RegisteredPlayers)
                {
                    udpStates.Add(player.UdpState);
                }
            }

            return udpStates;
        }

        public void WaitForRegistration()
        {
            Logger.Log(string.Format("The {0} server is listening on {1}.", Config.ServerName, LocalIPAddress + ":" + Config.ServerPort));
            Logger.Log("Please register for the game.");
        }

        /// <summary>
        /// Send message to one endPoint and wait for an answer.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <param name="endPoint"></param>
        public void SendAndWaitForAnswer(string message, Guid token, UdpState udpState)
        {
            byte[] messageBytes = GetByteArray(message);

            udpState.UdpClient.Send(messageBytes, messageBytes.Length, udpState.EndPoint);

            var timeTracker = new TimeOutTracker(Config.JoinTimeOut);
            udpState.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);

            while (timeTracker.IsValid)
            {
                Thread.Sleep(20);
            }
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
                miaServer.HandleReceivedRawMessage(receivedValue, udpStateClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Convert string to byte[]
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private byte[] GetByteArray(string message)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(message);
        }

        public void SendInfo(string message, UdpState udpState)
        {
            SendInfo(message, new List<UdpState> { udpState });
        }

        /// <summary>
        /// Send a one way message to end points
        /// </summary>
        /// <param name="message"></param>
        /// <param name="endPoints"></param>
        public void SendInfo(string message, List<UdpState> udpStates)
        {
            byte[] messageBytes = GetByteArray(message);

            foreach (UdpState udpState in udpStates)
            {
                udpState.UdpClient.Send(messageBytes, messageBytes.Length, udpState.EndPoint);
            }
        }

        #endregion Methods
    }
}
     