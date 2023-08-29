using System;
using System.Text;

using Game.Mia.Bot.Nightmare.Logging;

using LiteNetLib;


namespace Game.Mia.Bot.Nightmare.Game
{
    public class GameLogic
    {
        private Dicer lastDice = null;

        public void SendRegisterClient(NetPeer peer)
        {
            string playerName = Config.Config.Settings.PlayerName;
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

                    lastDice = Dicer.Parse(dice);
                    var nextDice = Dicer.Beat(lastDice);

                    messageResponse = "ANNOUNCE;" + token + ";" + nextDice;
                    break;
            }

            SendEvent(messageResponse, peer);
        }
    }
}
