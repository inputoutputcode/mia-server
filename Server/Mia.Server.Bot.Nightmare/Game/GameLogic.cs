using System.Text;

using Mia.Server.Bot.Nightmare.Configuration;
using Mia.Server.Bot.Nightmare.Logging;

using LiteNetLib;


namespace Mia.Server.Bot.Nightmare.Game
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

        public void ProcessEvent(string message, NetPeer peer)
        {
            string[] messageParts = message.Split(';');
            string commandText = string.Empty;
            string token = string.Empty;
            string dice = string.Empty;

            switch (messageParts[0])
            {
                case "REGISTERED":
                    break;

                case "ROUND_STARTING":
                    token = messageParts[1];
                    commandText = "JOIN_ROUND;" + token;
                    SendEvent(commandText, peer);
                    break;

                case "YOUR_TURN":
                    token = messageParts[1];
                    commandText = "ROLL;" + token;
                    SendEvent(commandText, peer);
                    break;

                case "ROLLED":
                    token = messageParts[1];
                    dice = messageParts[2];
                    commandText = "ANNOUNCE;" + token + ";" + dice;
                    SendEvent(commandText, peer);
                    break;
            }

            Log.Write(message);
        }
    }
}
