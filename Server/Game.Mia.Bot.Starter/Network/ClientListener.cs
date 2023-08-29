using System.Text;
using System.Net.Sockets;
using System.Net;

using Game.Server.Bot.Starter.Configuration;
using Game.Server.Bot.Starter.Logging;
using Game.Server.Bot.Starter.Game;

using LiteNetLib;


namespace Game.Server.Bot.Starter.Network
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

            Log.Write($"OnNetworkReceive: {peer.EndPoint.Address}:{peer.EndPoint.Port} - Message: {eventMessage} - ChannelNumber: {channelNumber} - DeliveryMethod: {deliveryMethod}");

            gameLogic.ProcessEvent(eventMessage, peer);
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

            if (request.RemoteEndPoint.Address.ToString() == Config.Settings.ServerAddress
                && request.RemoteEndPoint.Port == Config.Settings.ServerPort)
            {
                var peer = request.Accept();
            }
        }
    }
}
