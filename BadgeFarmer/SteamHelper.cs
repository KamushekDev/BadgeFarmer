using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
using BadgeFarmer.Models;
using BadgeFarmer.Models.Responses;
using Microsoft.OpenApi.Expressions;
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
            var client = Bot.SteamConfiguration.GetAsyncWebAPIInterface(ISteamApps);
            var response = await client.CallAsync(HttpMethod.Get, "GetAppList", 2);

            return 0;
        }


        internal async Task<MarketSearchResponse> GetPrices(
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
    }
}