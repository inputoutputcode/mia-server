using System.Collections.Generic;
using System.Text;

using Game.Mia.Bot.Advanced.Configuration;
using Game.Mia.Bot.Advanced.Logging;

using LiteNetLib;


namespace Game.Mia.Bot.Advanced.Game
{
    public class GameLogic
    {
        private List<Dicer> diceHistory = new List<Dicer>();

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
                    token = messageParts[2];
                    dice = messageParts[1];

                    // Beat dice and announce
                    if (diceHistory.Count == 0)
                    {
                        var nextDice = Dicer.Beat(Dicer.Parse(dice));
                        messageResponse = "ANNOUNCE;" + nextDice + ";" + token;
                    }
                    else if (diceHistory.Count > 0)
                    {
                        var lastAnnouncedDice = diceHistory[diceHistory.Count - 1];
                        var nextDice = Dicer.Beat(lastAnnouncedDice);
                        messageResponse = "ANNOUNCE;" + nextDice + ";" + token;
                    }
                    
                    break;

                case "ANNOUNCED":
                    dice = messageParts[2];

                    // Track last dice announcement
                    var announcedDice = Dicer.Parse(dice);
                    diceHistory.Add(announcedDice);
                    break;
            }

            if (!string.IsNullOrEmpty(messageResponse))
                SendEvent(messageResponse, peer);
        }
    }
}
