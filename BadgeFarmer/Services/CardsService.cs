using System;
using System.IO;
using Newtonsoft.Json;
using BadgeFarmer.Clients;
using System.Threading.Tasks;
using BadgeFarmer.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ArchiSteamFarm;
using BadgeFarmer.Extensions;
using SteamUtils.Models;

namespace BadgeFarmer.Services;

public class CardsService : ICardsService
{
    private const string CacheName = "cards.cache";

    private readonly ICustomSteamClient Client;

    private readonly List<Card> CardsCache = new(170_000);

    private readonly string FullPath;

    private readonly Random Random = new();

    private int Sync = 0;

    public CardsService(ICustomSteamClient client, string cachePath)
    {
        Client = client;
        FullPath = Path.Combine(cachePath, CacheName);
    }

    public int CardsTotal => CardsCache.Count;

    public Task UpdateCache(IProgress<(int current, int total)> progress) =>
        SyncAction(async () =>
        {
            const int approximateStart = 14200;
            const int step = 100;

            try
            {
                CardsCache.Clear();

                //finding real start point
                var currentOffset = approximateStart;

                Log("Updating cache...");

                // int isStart;
                // do
                // {
                //     isStart = await IsStartHere(currentOffset);
                //     //-1 to avoid infinite loops on the edge  | 0000000 | !0!0!0!0 |
                //     currentOffset += (step - 1) * isStart;
                // } while (isStart != 0);

                int cardsCount = 0;
                const int total = 152828;

                var cardBatches = await Enumerable.Range(0, total / step).ConcurrentSelectAsync(async (x, t) =>
                {
                    var cards = await GetCards(x * step);
                    Interlocked.Add(ref cardsCount, cards.Results.Count);
                    progress.Report((cardsCount, total));
                    return cards;
                }, 5, CancellationToken.None);
                var cards = cardBatches.SelectMany(x => x.Results).Select(x => x.ToCard());

                CardsCache.AddRange(cards);

                // int total;
                // do
                // {
                //     var response = await GetCards(currentOffset, step);
                //     total = response.TotalCount;
                //     var cards = response.Results.Where(x => x.SellPrice > 0).Select(x => x.ToCard());
                //     CardsCache.AddRange(cards);
                //     progress.Report((currentOffset, total));
                //     currentOffset = Math.Min(currentOffset + step, total);
                // } while (currentOffset < total);
                //
                // progress.Report((currentOffset, total));

                Log($"Cache was updated with total of {CardsTotal} cards.");
            }
            catch (Exception e)
            {
                Log($"ERROR: Update cache operation has thrown an exception: {e.Message}");
            }
        });

    private async Task<int> IsStartHere(int offset)
    {
        var response = await GetCards(offset);

        var firstPrice = response.Results.First().SellPrice;
        var lastPrice = response.Results.Last().SellPrice;

        var result = (firstPrice, lastPrice) switch
        {
            (0, 0) => 1,
            (> 0, > 0) => -1,
            (0, > 0) => 0,
            _ => throw new NotSupportedException("Cards weren't sorted by price in ascending order.")
        };
        return result;
    }

    private async Task<SearchMarketResponse> GetCards(int offset, int count = 100)
    {
        SearchMarketResponse response;
        var tryNumber = 0;
        do
        {
            try
            {
                var request = new SearchMarketRequest()
                {
                    Start = offset,
                    Count = count
                };
                response = await Client.GetPrices(request);

                await Task.Delay(GetDelayForTry(tryNumber));
                if (tryNumber > 3)
                    Log($"WARNING: SearchMarketRequest fails {tryNumber} times.");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                response = new SearchMarketResponse(false, 0, 0, 0, null, null);
                await Task.Delay(GetDelayForTry(tryNumber, e));
            }
            finally
            {
                tryNumber++;
            }
        } while (!response.Success || response.TotalCount == 0);

        return response;
    }

    public Task SaveCache() =>
        SyncAction(async () =>
        {
            var json = JsonConvert.SerializeObject(CardsCache);

            var file = File.OpenWrite(FullPath);

            await using var sw = new StreamWriter(file);

            await sw.WriteAsync(json);
        });

    public Task<bool> LoadCache() =>
        SyncAction(async () =>
        {
            CardsCache.Clear();

            try
            {
                var file = File.OpenRead(FullPath);

                using var sr = new StreamReader(file);

                var json = await sr.ReadToEndAsync();

                var list = JsonConvert.DeserializeObject<List<Card>>(json);

                CardsCache.AddRange(list);

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        });


    private async Task<T> SyncAction<T>(Func<Task<T>> action)
    {
        if (Interlocked.CompareExchange(ref Sync, 1, 0) != 0)
        {
            Log("Can't load cache, because cache is updating right now.");
            return default;
        }

        try
        {
            return await action();
        }
        finally
        {
            Log("Action ended.");
            Interlocked.Exchange(ref Sync, 0);
        }
    }

    private async Task SyncAction(Func<Task> action)
    {
        if (Interlocked.CompareExchange(ref Sync, 1, 0) != 0)
        {
            Log("Can't load cache, because cache is updating right now.");
            return;
        }

        try
        {
            await action();
        }
        finally
        {
            Log("Action ended.");
            Interlocked.Exchange(ref Sync, 0);
        }
    }


    private TimeSpan GetDelayForTry(int tryNumber, Exception ex = null)
    {
        if (tryNumber == 0 && ex is null)
            return TimeSpan.FromMilliseconds(Random.Next(3000));

        if (ex is not null)
            return TimeSpan.FromMinutes(2);

        return TimeSpan.FromSeconds(Math.Pow(2, 3 + tryNumber));
    }


    private void Log(string message)
    {
        Console.WriteLine(message);
        ASF.ArchiLogger.LogGenericInfo(message);
    }
}