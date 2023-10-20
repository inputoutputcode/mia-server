using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting;


namespace Game.Cluster.ClientRegister.Interface
{
    public interface IClientRegisterService : IService
    {
        Task<string> ReceiveClientCommand();
    }
}