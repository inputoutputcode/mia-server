using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LiteNetLib;
using LiteNetLib.Utils;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Game.Cluster.Gateway.Config;


namespace Game.Cluster.Gateway
{
    public class UdpCommunicationListener : ICommunicationListener, IDisposable, INetEventListener
    {
        private readonly CancellationTokenSource processRequestsCancellation = new CancellationTokenSource();
        public int Port { get; set; }
        private NetManager server;
        private ServiceSettings settings;

        public UdpCommunicationListener(ServiceSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Stops the Server Ungracefully
        /// </summary>
        public void Abort()
        {
            StopServer();
        }

        /// <summary>
        /// Stops the Server Gracefully
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task for Asynchron usage</returns>
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopServer();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Free Resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes Configuration
        /// </summary>
        /// <param name="context">Code Package Activation Context</param>
        public void Initialize(ICodePackageActivationContext context)
        {
            var serviceEndpoint = context.GetEndpoint("ServiceEndpoint");
            Port = serviceEndpoint.Port;
        }

        /// <summary>
        /// Starts the Server
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task for Asynchron usage</returns>
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                server = new NetManager(this)
                {
                    AutoRecycle = true,
                    DisconnectTimeout = settings.ClientDisconnectTimeoutInMs
                };
                server.Start(Port);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message(ex.Message);
            }

            RunServerInfiniteAsync(this.processRequestsCancellation.Token);

            return Task.FromResult("udp://" + FabricRuntime.GetNodeContext().IPAddressOrFQDN + ":" + this.Port);
        }

        private async void RunServerInfiniteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    server.PollEvents();
                    Thread.Sleep(15);
                }

                server.Stop();
            });
        }

        /// <summary>
        /// Free Resources and Stop Server
        /// </summary>
        /// <param name="disposing">Disposing .NET Resources?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.server != null)
                {
                    try
                    {
                        this.server.Stop();
                        this.server = null;
                    }
                    catch (Exception ex)
                    {
                        ServiceEventSource.Current.Message(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Stops Server and Free Handles
        /// </summary>
        private void StopServer()
        {
            this.processRequestsCancellation.Cancel();
            this.Dispose();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            ServiceEventSource.Current.Message($"OnPeerConnected: {peer.EndPoint.Address}:{peer.EndPoint.Port}");

            var writer = new NetDataWriter();
            writer.Put("Connection accepted");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServiceEventSource.Current.Message($"OnPeerDisconnected: {peer.EndPoint.Address}:{peer.EndPoint.Port} - DisconnectInfo: {disconnectInfo.SocketErrorCode} {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            ServiceEventSource.Current.Message($"OnNetworkError: {endPoint.Address}:{endPoint.Port} - {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            byte[] eventMessageBytes = reader.GetRemainingBytes();
            string eventMessage = Encoding.UTF8.GetString(eventMessageBytes);

            //Log.Write($"OnNetworkReceive: {peer.EndPoint.Address}:{peer.EndPoint.Port} - Message: {eventMessage} - ChannelNumber: {channelNumber} - DeliveryMethod: {deliveryMethod}");
            //Log.Write($"OnNetworkReceive: {eventMessage}");

            //gameManager.ReceiveEventMessage(eventMessage, peer);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            ServiceEventSource.Current.Message($"OnNetworkReceiveUnconnected: {remoteEndPoint.Address}:{remoteEndPoint.Port} - Message: {reader.GetString(20)} - MessageType: {messageType}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            if (latency > 100)
                ServiceEventSource.Current.Message($"OnNetworkLatencyUpdate: {peer.EndPoint.Address}:{peer.EndPoint.Port} - Latency: {latency}");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            ServiceEventSource.Current.Message($"OnConnectionRequest: {request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}");

            if (server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey(settings.ClientConnectionKey);
            else
                request.Reject();
        }
    }
}