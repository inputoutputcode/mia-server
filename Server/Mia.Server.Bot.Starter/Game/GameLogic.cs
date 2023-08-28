using LiteNetLib;
using Mia.Server.Bot.Starter.Configuration;
using Mia.Server.Bot.Starter.Logging;
using System.Text;


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

        public void ProcessEvent(string eventMessage, NetPeer peer)
        {
            Log.Write(eventMessage);

            string[] messageParts = eventMessage.Split(';');
            string message = string.Empty;
            string token = string.Empty;
            string dice = string.Empty;

            switch (messageParts[0])
            {
                case "REGISTERED":
                    break;

                case "ROUND_STARTING":
                    token = messageParts[1];
                    message = "JOIN;" + token;
                    break;

                case "YOUR_TURN":
                    token = messageParts[1];
                    message = "ROLL;" + token;
                    break;

                case "ROLLED":
                    token = messageParts[1];
                    dice = messageParts[2];
                    message = "ANNOUNCE;" + token + ";" + dice;
                    break;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
        }
    }
}
