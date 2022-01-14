using SteamUtils.Models;

namespace SteamUtils;

public class SteamApiClient : ISteamApiClient
{
    private readonly ISteamApi _api;

    public SteamApiClient(ISteamApi api)
    {
        _api = api;
    }

    public Task<SearchMarketResponse> SearchMarket(SearchMarketRequest request)
    {
        return _api.MarketSearch(request.Query,
            request.Game,
            request.ItemClass,
            request.AppId,
            request.Start.ToString(),
            request.Count.ToString(),
            request.SortColumn,
            request.SortDirection);
    }
}