using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Json;
using BadgeFarmer.Core.Models;
using BadgeFarmer.Extensions;
using BadgeFarmer.Models.Responses;
using RestEase;
using SteamKit2;
using SteamUtils;
using SteamUtils.Models;

namespace BadgeFarmer.Clients
{
    public class CustomSteamClient : ICustomSteamClient
    {
        private readonly Bot _bot;
        private readonly SteamApiClient _client;

        public const string SteamApiUrl = "https://api.steampowered.com";
        public const string SteamCommunityUrl = "https://steamcommunity.com";

        private const string EconService = "IEconService";
        private const string PlayerService = "IPlayerService";
        private const string SteamApps = "ISteamApps";
        private const string SteamUserAuth = "ISteamUserAuth";
        private const string TwoFactorService = "ITwoFactorService";
        private const string SteamCommunityHost = "steamcommunity.com";
        private const string SteamHelpHost = "help.steampowered.com";
        private const string SteamStoreHost = "store.steampowered.com";

        public CustomSteamClient(Bot bot)
        {
            _bot = bot;
            _client = new SteamApiClient(RestClient.For<ISteamApi>(SteamCommunityUrl));
        }

        public async Task<BadgesResponse> GetBadges()
        {
            const string getBadges = "GetBadges";

            var client = _bot.SteamConfiguration.GetAsyncWebAPIInterface(PlayerService);
            var (success, key) = await _bot.ArchiWebHandler.CachedApiKey.GetValue();
            if (!success)
                throw new Exception("Get api key wasn't successful :c");

            var response = await client.CallAsync(HttpMethod.Get, getBadges,
                args: new Dictionary<string, object>
                {
                    { "input_json", $"{{\"steamid\":\"{_bot.SteamID}\"}}" },
                    { "key", key! }
                });

            try
            {
                var badgesResponse = response.As<ResponseWrapper<BadgesResponse>>().Response;
                return badgesResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> GetGames()
        {
            throw new NotImplementedException();
            var client = _bot.SteamConfiguration.GetAsyncWebAPIInterface(SteamApps);
            var response = await client.CallAsync(HttpMethod.Get, "GetAppList", 2);

            return 0;
        }


        public async Task<SearchMarketResponse> GetPrices(SearchMarketRequest request)
        {
            var queryParams = request.GetQueryParams();

            // var response =
            //     await Bot.ArchiWebHandler.UrlGetToJsonObjectWithSession<MarketSearchResponse>(
            //         "https://steamcommunity.com", queryParams);
            //
            //
            // if (response?.Content != null)
            //     return response.Content;

            var response = await _client.SearchMarket(request);

            return response;
        }


        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&start=1&sort_column=price&sort_dir=desc&count=500&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=tag_app_730&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        internal async Task GetCardPrices() { }

        public async Task<ICollection<Steam.Asset>> GetInventory()
        {
            return await _bot.ArchiWebHandler.GetInventoryAsync(_bot.SteamID).ToListAsync();
        }
    }
}