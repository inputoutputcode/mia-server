using Microsoft.ServiceFabric.Services.Remoting;


namespace Game.Cluster.GameManager.Interface
{
    public interface IGameManagerService : IService
    {
        Task<string> RegisterPlayer(string playerName, IClient client);

        Task<Guid> CreateGame();
    }
}
