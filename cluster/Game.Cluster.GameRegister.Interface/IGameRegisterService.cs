using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting;


namespace Game.Cluster.GameRegister.Interface
{
    public interface IGameRegisterService : IService
    {
        Task<Guid> FindGameByName();

        Task<Guid> CreateGame();

        Task<Guid> RegisterPlayer();

    }
}
