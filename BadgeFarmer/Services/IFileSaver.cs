using System.Threading.Tasks;

namespace BadgeFarmer.Services;

public interface IFileSaver
{
    Task SaveAsync<T>(string filename, T content) where T : class;
    Task<T> LoadAsync<T>(string filename);
}