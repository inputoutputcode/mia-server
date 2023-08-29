using System.Net;
using System.Net.Sockets;
using System.Text;

using Mia.Server.ConsoleRunner.Configuration;
using Mia.Server.Game.Register.Interface;
using Mia.Server.Game.Monitoring;

using LiteNetLib;
using LiteNetLib.Utils;


namespace Mia.Server.Game.Communication
{
    public class ServerListener : INetEventListener
    {
        private readonly IGameManager gameManager;

        private NetManager server;

        public NetManager Server
        {
            set { server = value; }
        }

        public ServerListener(IGameManager gameManager) 
        { 
            this.gameManager = gameManager;
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Log.Write($"OnPeerConnected: {peer.EndPoint.Address}:{peer.EndPoint.Port}");

            var writer = new NetDataWriter();
            writer.Put("Connection accepted");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
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

            gameManager.ProcessEvent(eventMessage, peer);
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

            if (server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey(Config.Settings.ConnectionKey);
            else
                request.Reject();
        }
    }
}
