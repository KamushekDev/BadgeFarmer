using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BadgeFarmer.Core.Models;

namespace BadgeFarmer.Services;

public interface ICardsService
{
    Task UpdateCache(IProgress<(int current, int total)> progress);
    Task SaveCache();
    Task<bool> LoadCache();
    int CardsTotal { get; }
    IReadOnlyCollection<Card> Cards { get; }
}