using SteamUtils.Models;

namespace SteamUtils;

public interface ISteamApiClient
{
    Task<SearchMarketResponse> SearchMarket(SearchMarketRequest request);
}