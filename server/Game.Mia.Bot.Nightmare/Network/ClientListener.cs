using System.Net;
using System.Net.Sockets;
using System.Text;

using Game.Mia.Bot.Nightmare.Game;
using Game.Mia.Bot.Nightmare.Logging;

using LiteNetLib;


namespace Game.Mia.Bot.Nightmare.Network
{
    public class ClientListener : INetEventListener
    {
        private GameLogic gameLogic;

        public ClientListener(GameLogic gameLogic)
        {
            this.gameLogic = gameLogic;
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Log.Write($"OnPeerConnected: {peer.EndPoint.Address}:{peer.EndPoint.Port}");

            gameLogic.SendRegisterClient(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Write($"OnPeerDisconnected: {peer.EndPoint.Address}:{peer.EndPoint.Port} - DisconnectInfo: {disconnectInfo.SocketErrorCode} {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Write($"OnNetworkError: {endPoint.Address}:{endPoint.Port} - {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            byte[] eventMessageBytes = reader.GetRemainingBytes();
            string eventMessage = Encoding.UTF8.GetString(eventMessageBytes);

            Log.Write($"OnNetworkReceive: {eventMessage}");

            string messageResponse = gameLogic.ProcessEvent(eventMessage);
            if (!string.IsNullOrEmpty(messageResponse))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageResponse);
                peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);

                Log.Write(messageResponse);
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Log.Write($"OnNetworkReceiveUnconnected: {remoteEndPoint.Address}:{remoteEndPoint.Port} - Message: {reader.GetString(20)} - MessageType: {messageType}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            if (latency > 100)
                Log.Write($"OnNetworkLatencyUpdate: {peer.EndPoint.Address}:{peer.EndPoint.Port} - Latency: {latency}");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Log.Write($"OnConnectionRequest: {request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}");

            if (request.RemoteEndPoint.Address.ToString() == Config.Config.Settings.ServerAddress
                && request.RemoteEndPoint.Port == Config.Config.Settings.ServerPort)
            {
                var peer = request.Accept();
            }
        }
    }
}
