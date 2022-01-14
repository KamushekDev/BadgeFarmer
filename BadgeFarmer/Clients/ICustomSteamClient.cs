using System.Collections.Generic;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using BadgeFarmer.Models.Responses;
using SteamUtils.Models;

namespace BadgeFarmer.Clients;

public interface ICustomSteamClient
{
    Task<BadgesResponse> GetBadges();
    Task<int> GetGames();
    Task<SearchMarketResponse> GetPrices(SearchMarketRequest request);
    Task<ICollection<Steam.Asset>> GetInventory();
}