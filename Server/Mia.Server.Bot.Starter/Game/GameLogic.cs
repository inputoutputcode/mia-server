﻿using System.Text;

using Mia.Server.Bot.Starter.Configuration;
using Mia.Server.Bot.Starter.Logging;

using LiteNetLib;


namespace Mia.Server.Bot.Starter.Game
{
    public class GameLogic
    {
        public void SendRegisterClient(NetPeer peer)
        {
            string playerName = Config.Settings.PlayerName;
            string message = "REGISTER;" + playerName;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
        }

        public void SendEvent(string message, NetPeer peer)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);

            Log.Write(message);
        }

        public void ProcessEvent(string eventMessage, NetPeer peer)
        {
            string[] messageParts = eventMessage.Split(';');
            string messageResponse = string.Empty;
            string token;
            string dice;

            switch (messageParts[0])
            {
                case "REGISTERED":
                    break;

                case "ROUND_STARTING":
                    token = messageParts[1];
                    messageResponse = "JOIN_ROUND;" + token;
                    break;

                case "YOUR_TURN":
                    token = messageParts[1];
                    messageResponse = "ROLL;" + token;
                    break;

                case "ROLLED":
                    token = messageParts[1];
                    dice = messageParts[2];
                    messageResponse = "ANNOUNCE;" + token + ";" + dice;
                    break;
            }

            SendEvent(messageResponse, peer);
        }
    }
}
