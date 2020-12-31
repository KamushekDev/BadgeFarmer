using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
using BadgeFarmer.Models;
using BadgeFarmer.Models.Responses;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace BadgeFarmer
{
    internal sealed class SteamHelper
    {
        private readonly Bot Bot;

        public const string SteamApiUrl = "https://api.steampowered.com/";

        private const string IEconService = "IEconService";
        private const string IPlayerService = "IPlayerService";
        private const string ISteamApps = "ISteamApps";
        private const string ISteamUserAuth = "ISteamUserAuth";
        private const string ITwoFactorService = "ITwoFactorService";
        private const string SteamCommunityHost = "steamcommunity.com";
        private const string SteamHelpHost = "help.steampowered.com";
        private const string SteamStoreHost = "store.steampowered.com";

        public SteamHelper(Bot bot)
        {
            Bot = bot;
        }

        internal async Task<BadgesResponse> GetBadges()
        {
            const string getBadges = "GetBadges";

            var client = Bot.SteamConfiguration.GetAsyncWebAPIInterface(IPlayerService);
            var (success, key) = await Bot.ArchiWebHandler.CachedApiKey.GetValue();
            if (!success)
                throw new Exception();
            var response = await client.CallAsync(HttpMethod.Get, getBadges,
                args: new Dictionary<string, object>
                {
                    {"input_json", $"{{\"steamid\":\"{Bot.SteamID}\"}}"},
                    {"key", key!}
                });

            try
            {
                var badgesResponse = response.As<ResponseWrapper<BadgesResponse>>().Response;
                return badgesResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        internal async Task<int> GetGames()
        {
            //todo
            var client = Bot.SteamConfiguration.GetAsyncWebAPIInterface(ISteamApps);
            var response = await client.CallAsync(HttpMethod.Get, "GetAppList", 2);

            return 0;
        }

        internal async Task<ItemPrice> PriceOverview(
            int appId,
            string marketHashName,
            string country = "US",
            ECurrencyCode currency = ECurrencyCode.RUB)
        {
            //https://steamcommunity.com/market/priceoverview/?country=US&currency=5&appid=753&market_hash_name=336940-Bankers
            var paramsString =
                $"country={country}&currency={(int) currency}&appid={appId}&market_hash_name={marketHashName}";
            var response =
                await Bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<ItemPrice>(
                    "https://steamcommunity.com",
                    $"/market/priceoverview/?{paramsString}");


            if (response?.Content != null)
                return response.Content;
            throw new Exception();
        }

        internal async Task<MarketSearchResponse> QueryMarket(
            string query = "",
            string itemClass = "tag_item_class_2",
            string sortColumn = "price",
            string sortDir = "desc",
            string appid = "753",
            string game = "any",
            int start = 1,
            int count = 100)
        {
            var searchParams =
                $"q={query}&category_753_Game%5B%5D={game}&category_753_item_class%5B%5D={itemClass}&appid={appid}";
            var pagingParams =
                $"start={start}&count={count}&sort_column={sortColumn}&sort_dir={sortDir}";

            var response =
                await Bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<MarketSearchResponse>(
                    "https://steamcommunity.com",
                    $"/market/search/render/?{searchParams}&{pagingParams}&norender=1");


            if (response?.Content != null)
                return response.Content;
            throw new Exception();
        }


        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&start=1&sort_column=price&sort_dir=desc&count=500&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=tag_app_730&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        internal async Task GetCardPrices()
        {
        }

        // POST /market/createbuyorder/
        // Request Data
        // MIME Type: application/x-www-form-urlencoded; charset=UTF-8
        // sessionid: c38fa822fa344870d12326f2
        //     currency: 5
        // appid: 753
        // market_hash_name: 485830-Ships
        //     price_total: 221
        // quantity: 1
        // billing_state
        //     save_my_address: 0
        internal async Task CreateBuyOrder(ECurrencyCode currency = ECurrencyCode.RUB)
        {
            var a =
                await Bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<JObject>(
                    SteamCommunityHost,
                    "/market/createbuyorder/",
                    data: new List<KeyValuePair<string, string>>
                    {
                        new("currency", ((int) currency).ToString()),
                        new("appid", "753"),
                        new("market_hash_name", "485830-Ships"),
                        new("price_total", "222"),
                        new("quantity", "1"),
                        new("save_my_address", "0")
                    });
        }
    }
}