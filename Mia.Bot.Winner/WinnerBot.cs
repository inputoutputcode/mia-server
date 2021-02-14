using System;
using System.Configuration;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Mia.Bot.Winner
{
    public class WinnerBot
    {
        public UdpState udpStateServer;

        public void Connect()
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

            udpStateServer = new UdpState
            {
                EndPoint = serverEndPoint,
                UdpClient = udpClient
            };
            string playerName = ConfigurationManager.AppSettings["PlayerName"];
            SendMessageToServer("REGISTER;" + playerName, udpClient);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallBack), udpStateServer);
        }

        public void SendMessageToServer(string message, UdpClient udpClient)
        {
            var encoding = new UTF8Encoding();
            byte[] messageBytes = encoding.GetBytes(message);

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

            switch (messageParts[0])
            {
                case "ROUND_STARTING":

                    SendMessageToServer("JOIN;" + messageParts[1], client);
                    break;

                case "YOUR_TURN":

                    SendMessageToServer("ROLL;" + messageParts[1], client);
                    break;

                case "ROLLED":

                    SendMessageToServer("ANNOUNCE;" + messageParts[1] + ";" + messageParts[2], client);
                    break;
            }

            Console.WriteLine(receivedValue);
        }
    }
}
