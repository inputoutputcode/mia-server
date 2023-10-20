using System;
using System.Threading.Tasks;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

using Game.Cluster.GameManager.Interface;


namespace Game.Cluster.GameManager
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class GameManagerService : StatefulService, IGameManagerService
    {
        public GameManagerService(StatefulServiceContext context)
            : base(context)
        { }

        public Task<Guid> CreateGame()
        {
            throw new NotImplementedException();
        }

        public Task<string> RegisterPlayer(string playerName, IClient client)
        {
            throw new NotImplementedException();
        }
    }
}
