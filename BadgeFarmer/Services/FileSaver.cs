using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BadgeFarmer.Services;

public class FileSaver : IFileSaver
{
    private readonly string _folderPath = ArchiSteamFarm.SharedInfo.ConfigDirectory;

    //todo: thread safety

    public async Task SaveAsync<T>(string filename, T content)
        where T : class
    {
        var json = JsonConvert.SerializeObject(content, Formatting.None);

        var fullPath = Path.Combine(_folderPath, filename);

        var file = File.Open(fullPath, FileMode.Create);

        await using var sw = new StreamWriter(file);

        await sw.WriteAsync(json);
    }

    public async Task<T> LoadAsync<T>(string filename)
    {
        try
        {
            var fullPath = Path.Combine(_folderPath, filename);
            var file = File.OpenRead(fullPath);

            using var sr = new StreamReader(file);

            var json = await sr.ReadToEndAsync();

            var content = JsonConvert.DeserializeObject<T>(json);

            return content;
        }
        catch (FileNotFoundException)
        {
            return default;
        }
    }

    // private async Task SyncAction(Func<Task> action)
    // {
    //     if (Interlocked.CompareExchange(ref Sync, 1, 0) != 0)
    //     {
    //         Log("Can't load cache, because cache is updating right now.");
    //         return;
    //     }
    //
    //     try
    //     {
    //         await action();
    //     }
    //     finally
    //     {
    //         Log("Action ended.");
    //         Interlocked.Exchange(ref Sync, 0);
    //     }
    // }
    // private async Task<T> SyncAction<T>(Func<Task<T>> action)
    // {
    //     if (Interlocked.CompareExchange(ref Sync, 1, 0) != 0)
    //     {
    //         Log("Can't load cache, because cache is updating right now.");
    //         return default;
    //     }
    //
    //     try
    //     {
    //         return await action();
    //     }
    //     finally
    //     {
    //         Log("Action ended.");
    //         Interlocked.Exchange(ref Sync, 0);
    //     }
    // }
}