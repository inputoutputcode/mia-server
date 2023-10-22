using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Game.Cluster.GameManager.Interface;


namespace Game.Cluster.GameManager
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class GameManagerService : StatefulService, IGameManagerService
    {
        public GameManagerService(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }


        public Task<Guid> CreateGame()
        {
            return Task.FromResult(Guid.NewGuid());
        }

        public Task<bool> RegisterPlayer(string playerName, IClient client)
        {
            return Task.FromResult(true);
        }
    }
}
