using Microsoft.ServiceFabric.Services.Remoting;


namespace Game.Cluster.GameManager.Interface
{
    public interface IGameManagerService : IService
    {
        Task<bool> RegisterPlayer(string playerName);

        Task<Guid> CreateGame();
    }
}
