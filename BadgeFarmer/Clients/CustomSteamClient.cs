using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
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
        private readonly Bot Bot;
        private readonly SteamApiClient Client;

        public const string SteamApiUrl = "https://api.steampowered.com";
        public const string SteamCommunityUrl = "https://steamcommunity.com";

        private const string IEconService = "IEconService";
        private const string IPlayerService = "IPlayerService";
        private const string ISteamApps = "ISteamApps";
        private const string ISteamUserAuth = "ISteamUserAuth";
        private const string ITwoFactorService = "ITwoFactorService";
        private const string SteamCommunityHost = "steamcommunity.com";
        private const string SteamHelpHost = "help.steampowered.com";
        private const string SteamStoreHost = "store.steampowered.com";

        public CustomSteamClient(Bot bot)
        {
            Bot = bot;
            Client = new SteamApiClient(RestClient.For<ISteamApi>(SteamCommunityUrl));
        }

        public async Task<BadgesResponse> GetBadges()
        {
            const string getBadges = "GetBadges";

            var client = Bot.SteamConfiguration.GetAsyncWebAPIInterface(IPlayerService);
            var (success, key) = await Bot.ArchiWebHandler.CachedApiKey.GetValue();
            if (!success)
                throw new Exception("Get api key wasn't successful :c");

            var response = await client.CallAsync(HttpMethod.Get, getBadges,
                args: new Dictionary<string, object>
                {
                    { "input_json", $"{{\"steamid\":\"{Bot.SteamID}\"}}" },
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
            var client = Bot.SteamConfiguration.GetAsyncWebAPIInterface(ISteamApps);
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

            var response = await Client.SearchMarket(request);

            return response;
        }


        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&start=1&sort_column=price&sort_dir=desc&count=500&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=any&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        //https://steamcommunity.com/market/search/render/?q=&category_753_Game%5B%5D=tag_app_730&category_753_item_class%5B%5D=tag_item_class_2&appid=753&norender=1
        internal async Task GetCardPrices() { }
    }
}