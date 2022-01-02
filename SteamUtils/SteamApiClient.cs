using SteamUtils.Models;

namespace SteamUtils;

public class SteamApiClient : ISteamApiClient
{
    private readonly ISteamApi Api;

    public SteamApiClient(ISteamApi api)
    {
        Api = api;
    }

    public Task<SearchMarketResponse> SearchMarket(SearchMarketRequest request)
    {
        return Api.MarketSearch(request.Query,
            request.Game,
            request.ItemClass,
            request.AppId,
            request.Start.ToString(),
            request.Count.ToString(),
            request.SortColumn,
            request.SortDirection);
    }
}