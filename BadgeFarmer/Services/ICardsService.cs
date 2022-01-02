using System;
using System.Threading.Tasks;

namespace BadgeFarmer.Services;

public interface ICardsService
{
    Task UpdateCache(IProgress<(int current, int total)> progress);
    Task SaveCache();
    Task<bool> LoadCache();
    int CardsTotal { get; }
}