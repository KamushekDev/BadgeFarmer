using System.Threading.Tasks;
using BadgeFarmer.Models.Responses;
using SteamUtils.Models;

namespace BadgeFarmer.Clients;

public interface ICustomSteamClient
{
    Task<BadgesResponse> GetBadges();
    Task<int> GetGames();
    Task<SearchMarketResponse> GetPrices(SearchMarketRequest request);
}