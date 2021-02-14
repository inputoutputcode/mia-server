using System;
using System.Configuration;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Mia.Bot.Starter
{
    public class StarterBot
    {
        public UdpState udpStateServer;

        public StarterBot()
        {
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"];
            string serverPort = ConfigurationManager.AppSettings["ServerPort"];

            var serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), int.Parse(serverPort));
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var localIpAddress = (from address in hostEntry.AddressList where address.AddressFamily == AddressFamily.InterNetwork select address.ToString()).FirstOrDefault();
            var localEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), int.Parse(ConfigurationManager.AppSettings["LocalPort"]));

            Thread.Sleep(3000);

            var udpClient = new UdpClient(localEndPoint);
            udpClient.Connect(serverEndPoint);

            string playerName = ConfigurationManager.AppSettings["PlayerName"];
            string commandText = "REGISTER;" + playerName;
            SendCommand(commandText, udpClient);

            udpStateServer = new UdpState
            {
                EndPoint = serverEndPoint,
                UdpClient = udpClient
            };
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallBack), udpStateServer);
        }

        public void SendCommand(string message, UdpClient udpClient)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(messageBytes, messageBytes.Length);

            Console.WriteLine(message);
        }

        public void ReceiveCallBack(IAsyncResult result)
        {
            UdpClient client = ((UdpState)(result.AsyncState)).UdpClient;
            IPEndPoint endPoint = ((UdpState)(result.AsyncState)).EndPoint;

            byte[] receivedBytes = client.EndReceive(result, ref endPoint);
            client.BeginReceive(new AsyncCallback(ReceiveCallBack), udpStateServer);
            string receivedValue = Encoding.UTF8.GetString(receivedBytes);
            string[] messageParts = receivedValue.Split(';');
            string commandText = string.Empty;
            string token = string.Empty;
            string dice = string.Empty;

            switch (messageParts[0])
            {
                case "REGISTERED":
                    break;

                case "ROUND_STARTING":
                    token = messageParts[1];
                    commandText = "JOIN;" + token;
                    SendCommand(commandText, client);
                    break;

                case "YOUR_TURN":
                    token = messageParts[1];
                    commandText = "ROLL;" + token;
                    SendCommand(commandText, client);
                    break;

                case "ROLLED":
                    token = messageParts[1];
                    dice = messageParts[2];
                    commandText = "ANNOUNCE;" + token + ";" + dice;
                    SendCommand(commandText, client);
                    break;
            }

            Console.WriteLine(receivedValue);
        }
    }
}
