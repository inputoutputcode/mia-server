using System.Text;

using Game.Server.Spectator.Configuration;
using Game.Server.Spectator.Logging;

using LiteNetLib;


namespace Game.Server.Spectator.Game
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
        }

        public void ProcessEvent(string eventMessage, NetPeer peer)
        {
            string[] messageParts = eventMessage.Split(';');
            string token;
            string dice;

            switch (messageParts[0])
            {
                case "REGISTERED":
                    break;

                case "ROUND_STARTING":
                    token = messageParts[1];
                    string messageResponse = "JOIN_SPECTATOR;" + token;
                    SendEvent(messageResponse, peer);
                    break;

                case "ANNOUNCED":
                    dice = messageParts[2];
                    break;

                case "PLAYER_LOST":
                    break;

                case "SCORE":
                    string scores = messageParts[1];
                    token = messageParts[2];
                    Log.Write($"SCORE: {scores}");
                    break;
            }
        }
    }
}
