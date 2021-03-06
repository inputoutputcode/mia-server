﻿using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Mia.Server.Bot.Nightmare.Configuration;


namespace Mia.Server.Bot.Nightmare
{
    public class GamePlay
    {
        public UdpState udpStateServer;

        public IPAddress LocalIPAddress
        {
            get
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = (from address in hostEntry.AddressList where address.AddressFamily == AddressFamily.InterNetwork select address.ToString()).FirstOrDefault();

                return IPAddress.Parse(ipAddress);
            }
        }

        public GamePlay(bool isLocalServer = false)
        {
            string serverAddress = Config.Settings.ServerAddress;
            int serverPort = Config.Settings.ServerPort;

            if (isLocalServer)
                serverAddress = LocalIPAddress.ToString();

            var serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var localIpAddress = (from address in hostEntry.AddressList where address.AddressFamily == AddressFamily.InterNetwork select address.ToString()).FirstOrDefault();
            var localEndPoint = new IPEndPoint(IPAddress.Parse(localIpAddress), Config.Settings.LocalPort);

            Thread.Sleep(2000);

            var udpClient = new UdpClient(localEndPoint);
            udpClient.Connect(serverEndPoint);

            string playerName = Config.Settings.PlayerName;
            string commandText = "REGISTER;" + playerName;
            SendCommand(commandText, udpClient);

            udpStateServer = new UdpState
            {
                EndPoint = serverEndPoint,
                UdpClient = udpClient
            };
            udpStateServer.UdpClient.BeginReceive(new AsyncCallback(ReceiveCallBack), udpStateServer);
        }

        public void SendCommand(string message, UdpClient udpClient)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(messageBytes, messageBytes.Length);

            WriteLog(message);
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

            WriteLog(receivedValue);
        }

        public void WriteLog(string message)
        {
            string filteredMessage = Regex.Replace(message, @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}", "", RegexOptions.IgnoreCase);
            Console.WriteLine(message);
        }
    }
}
