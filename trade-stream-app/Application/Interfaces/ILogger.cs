using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILogger
    {
        Task LogToConsoleAsync(string message);
    }
}
