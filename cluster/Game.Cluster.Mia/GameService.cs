using System.Threading.Tasks;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

using Game.Cluster.Mia.Interface;


namespace Game.Cluster.Game
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class GameService : StatefulService, IGameService
    {
        public GameService(StatefulServiceContext context)
            : base(context)
        { }

        public Task<string> ProcessClientMove(IClientMove clientMove, IPlayer client)
        {
            throw new System.NotImplementedException();
        }
    }
}
