using RestEase;
using SteamUtils.Models;

namespace SteamUtils;

[Header("Host", "steamcommunity.com")]
[Header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36")]
public interface ISteamApi
{

    [Get(
        "market/search/render/?q={query}&category_753_Game%5B%5D={category}&category_753_item_class%5B%5D={itemClass}&appid={appId}&start={start}&count={count}&sort_column={sortColumn}&sort_dir={sortDirection}&norender=1")]
    Task<SearchMarketResponse> MarketSearch(
        [Path]
        string query,
        [Path]
        string category,
        [Path]
        string itemClass,
        [Path]
        string appId,
        [Path]
        string start,
        [Path]
        string count,
        [Path]
        string sortColumn,
        [Path]
        string sortDirection);
}