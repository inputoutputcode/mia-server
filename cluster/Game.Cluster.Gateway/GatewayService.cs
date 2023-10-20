using System.Collections.Generic;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

using Game.Cluster.Gateway.Config;
using Game.Cluster.Gateway.Logging;
using Game.Cluster.Gateway.Network;


namespace Game.Cluster.Gateway
{
    /// <summary>
    /// Duties of GatewayService
    /// FindGameByName -> GameRegister
    /// CreateGame -> GameRegister
    /// GameTotal -> GameRegister
    /// PlayerTotal -> GameRegister
    /// RegisterPlayer -> GameManager(Name, IPAddress)
    /// CreateGame -> GameRegister -> GameId
    /// JoinGame -> Game(by GameId, Token, RunMode, CompeteMode)
    /// AnyTurn -> Game(by GameId, Token)
    /// LeaveGame -> Game(by GameId, Token)
    /// </summary>
    internal sealed class GatewayService : StatelessService
    {
        private ServiceSettings settings;

        public GatewayService(StatelessServiceContext context)
            : base(context)
        {
            var activationContext = FabricRuntime.GetActivationContext();
            var configSettings = activationContext.GetConfigurationPackageObject("Config").Settings;
            var data = configSettings.Sections["Gateway.Settings"];

            foreach (var parameter in data.Parameters)
            {
                ServiceEventSource.Current.ServiceMessage(Context, "Working-{0} - {1}", parameter.Name, parameter.Value);
            }

            settings = new ServiceSettings(data);
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context => new UdpCommunicationListener(context, settings))
            };
        }
    }
}
