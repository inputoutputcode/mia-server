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

        public string ProcessEvent(string eventMessage)
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
                    diceHistory.Clear();
                    break;

                case "YOUR_TURN":
                    token = messageParts[1];

                    if (diceHistory.Count > 1)
                    {
                        var lastAnnouncedDice1 = diceHistory[diceHistory.Count - 1];
                        var lastAnnouncedDice2 = diceHistory[diceHistory.Count - 2];

                        // SEE if last dice was only one higher than the previous
                        if (diceHistory.Count > 1 && Dicer.BeatByOne(lastAnnouncedDice2).IsHigherThan(lastAnnouncedDice1) == 0)
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
                    Dicer lastAnnouncedDice = null;
                    Dicer rolledDice = null;
                    var randomizer = new Random();
                    int randomSecondRoll = randomizer.Next(0, 1);

                    if (diceHistory.Count > 0)
                        lastAnnouncedDice = diceHistory[diceHistory.Count - 1];

                    if (messageParts.Length == 2 && rollCounter == 2)
                    {
                        token = messageParts[1];
                        // Reset to announce if already second ROLL
                        randomSecondRoll = 0;
                    }
                    else
                    {
                        dice = messageParts[1];
                        token = messageParts[2];
                        rolledDice = Dicer.Parse(dice);
                    }

                    // Second roll if current dice lower than last announced dice
                    if (lastAnnouncedDice != null && rollCounter == 1 && lastAnnouncedDice.IsHigherThan(rolledDice) == 1)
                    {
                        randomSecondRoll = 1;
                    }

                    switch(randomSecondRoll)
                    {
                        case 0:
                            Dicer nextDice = null;

                            // Take rolled dice if higher than last one
                            if (diceHistory.Count == 0 || lastAnnouncedDice != null && rolledDice != null && rolledDice.IsHigherThan(lastAnnouncedDice) == 1)
                            {
                                nextDice = rolledDice;
                            }
                            else
                            {
                                nextDice = Dicer.Beat(lastAnnouncedDice);
                            }

                            messageResponse = "ANNOUNCE;" + nextDice + ";" + token;
                            rollCounter = 0;

                            break;

                        case 1:
                            messageResponse = "ROLL;" + token;
                            rollCounter = 2;

                            break;
                    }

                    
                    break;

                case "ANNOUNCED":
                    dice = messageParts[2];

                    // Track last dice announcement
                    var announcedDice = Dicer.Parse(dice);
                    if (announcedDice != null)
                        diceHistory.Add(announcedDice);
                    break;
            }

            return messageResponse;
        }
    }
}
