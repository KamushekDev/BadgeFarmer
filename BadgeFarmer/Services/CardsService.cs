using System;
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
    private const string CardsCacheName = "cards.cache";
    private const int ApproximateUpperBound = 170_000;

    private readonly ICustomSteamClient _client;
    private readonly IFileSaver _fileSaver;
    private readonly List<SearchMarketResponse.SearchItem> _cardsCache = new(ApproximateUpperBound);
    private readonly List<Card> _cards = new(ApproximateUpperBound);
    private readonly Random _random = new();

    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();
    public int CardsTotal => _cards.Count;

    public CardsService(ICustomSteamClient client, IFileSaver fileSaver)
    {
        _client = client;
        _fileSaver = fileSaver;
    }


    public async Task UpdateCache(IProgress<(int current, int total)> progress)
    {
        // const int approximateStart = 14200;
        const int step = 100;

        try
        {
            Log("Updating cache...");

            int cardsCount = 0;
            const int total = 158282;

            var cardBatches = await Enumerable.Range(0, total / step + 2).ConcurrentSelectAsync(async (x, t) =>
            {
                var cards = await GetCards(x * step);
                Interlocked.Add(ref cardsCount, cards.Results.Count);
                progress.Report((cardsCount, total));
                return cards;
            }, 5, CancellationToken.None);
            var cards = cardBatches.SelectMany(x => x.Results); //.Select(x => x.ToCard());

            _cardsCache.Clear();
            _cardsCache.AddRange(cards);

            Log($"Cache was updated with total of {_cardsCache.Count} cards.");
        }
        catch (Exception e)
        {
            Log($"ERROR: Update cache operation has thrown an exception: {e.Message}");
        }
    }

    //todo: currency
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
                response = await _client.GetPrices(request);

                await Task.Delay(GetDelayForTry(tryNumber));
                if (tryNumber > 3)
                    Log($"WARNING: SearchMarketRequest fails {tryNumber} times.");
            }
            catch (Exception e)
            {
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

    public Task SaveCache()
    {
        return _fileSaver.SaveAsync(CardsCacheName, _cardsCache);
    }

    public async Task<bool> LoadCache()
    {
        var cards = await _fileSaver.LoadAsync<List<SearchMarketResponse.SearchItem>>(CardsCacheName);
        if (cards is null)
            return false;

        _cardsCache.Clear();
        _cardsCache.AddRange(cards);

        _cards.Clear();
        _cards.AddRange(_cardsCache.Select(x => x.ToCard()));
        return true;
    }

    private TimeSpan GetDelayForTry(int tryNumber, Exception ex = null)
    {
        if (tryNumber == 0 && ex is null)
            return TimeSpan.FromMilliseconds(_random.Next(500));

        //if (ex is not null)
        return TimeSpan.FromMinutes(2);

        //return TimeSpan.FromSeconds(Math.Pow(2, 3 + tryNumber));
    }


    private void Log(string message)
    {
        Console.WriteLine(message);
        ASF.ArchiLogger.LogGenericInfo(message);
    }
}