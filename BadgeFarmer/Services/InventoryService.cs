using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using BadgeFarmer.Clients;
using BadgeFarmer.Models;
using SteamKit2.GC.Dota.Internal;

namespace BadgeFarmer.Services;

public class InventoryService : IInventoryService
{
    private readonly ICustomSteamClient _client;

    public InventoryService(ICustomSteamClient client)
    {
        _client = client;
    }

    public async Task<IList<AccountCard>> GetAccountCards()
    {
        var accountItems = await _client.GetInventory();
        var cards = accountItems.Where(x =>
                x.Type is Steam.Asset.EType.TradingCard or Steam.Asset.EType.FoilTradingCard)
            .Select(x => new AccountCard(x))
            .GroupBy(x => x.MarketHashName)
            .Select(x => x.Aggregate(AccountCard.Merge));

        return cards.ToList();
    }
}