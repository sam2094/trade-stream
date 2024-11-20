using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMarketDataService
    {
        Task StartAsync();
        Task StopAsync();
        void Dispose();
    }
}