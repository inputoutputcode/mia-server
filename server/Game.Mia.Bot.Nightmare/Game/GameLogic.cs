using System;
using System.Collections.Generic;
using System.Text;

using Game.Mia.Bot.Nightmare.Logging;

using LiteNetLib;


namespace Game.Mia.Bot.Nightmare.Game
{
    public class GameLogic
    {
        private List<Dicer> diceHistory = new List<Dicer>();
        private int rollCounter  = 0;

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

                    if (diceHistory.Count > 1)
                    {
                        var lastAnnouncedDice1 = diceHistory[diceHistory.Count - 1];
                        var lastAnnouncedDice2 = diceHistory[diceHistory.Count - 2];

                        // SEE if last dice was not higher than the previous
                        if (diceHistory.Count > 0 && lastAnnouncedDice2.CompareTo(lastAnnouncedDice1) < 1)
                        {
                            messageResponse = "SEE;" + token;
                        }
                    }
                    
                    // ROLL for new dice
                    if (string.IsNullOrEmpty(messageResponse))
                    {
                        messageResponse = "ROLL;" + token;
                        rollCounter = 1;
                    }

                    break;

                case "ROLLED":
                    token = messageParts[2];
                    dice = messageParts[1];

                    Dicer lastAnnouncedDice = null;

                    if (diceHistory.Count > 0)
                        lastAnnouncedDice = diceHistory[diceHistory.Count - 1];

                    var rolledDice = Dicer.Parse(dice);

                    var randomizer = new Random();
                    int randomSecondRoll = randomizer.Next(1, 2);

                    // Reset to announce if already second ROLL
                    if (rollCounter == 2)
                        randomSecondRoll = 1;

                    switch(randomSecondRoll)
                    {
                        case 1:
                            Dicer nextDice = null;

                            // Take rolled dice if higher than last one
                            if (diceHistory.Count > 0 && lastAnnouncedDice != null && lastAnnouncedDice.CompareTo(rolledDice) == -1)
                            {
                                nextDice = rolledDice;
                            }
                            // Beat last dice with randomized
                            else
                            {
                                if (diceHistory.Count == 0)
                                {
                                    int randomOne = randomizer.Next(1, 6);
                                    int randomTwo = randomizer.Next(1, 6);

                                    nextDice = new Dicer(randomOne, randomTwo);
                                }
                                else
                                {
                                    nextDice = Dicer.Beat(lastAnnouncedDice, true);
                                }
                            }

                            messageResponse = "ANNOUNCE;" + nextDice + ";" + token;
                            rollCounter = 0;

                            break;

                        case 2:
                            messageResponse = "ROLL;" + token;
                            rollCounter = 2;

                            break;
                    }

                    
                    break;

                case "ANNOUNCED":
                    dice = messageParts[2];

                    // Track last dice announcement
                    var announcedDice = Dicer.Parse(dice);
                    diceHistory.Add(announcedDice);
                    break;
            }

            SendEvent(messageResponse, peer);
        }
    }
}
