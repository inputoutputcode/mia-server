using Microsoft.ServiceFabric.Services.Remoting;


namespace Game.Cluster.Mia.Interface
{
    public interface IGameService : IService
    {
        Task<string> ProcessClientMove(IClientMove clientMove, IPlayer client);
    }
}