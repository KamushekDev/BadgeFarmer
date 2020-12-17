using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
using BadgeFarmer.Models;
using BadgeFarmer.Responses;
using Microsoft.OpenApi.Expressions;
using SteamKit2;

namespace BadgeFarmer
{
    internal sealed class SteamHelper
    {
        private readonly Bot Bot;
        private readonly bool DisposeWebHandler;

        public const string SteamApiUrl = "https://api.steampowered.com/";

        private const string IEconService = "IEconService";
        private const string IPlayerService = "IPlayerService";
        private const string ISteamApps = "ISteamApps";
        private const string ISteamUserAuth = "ISteamUserAuth";
        private const string ITwoFactorService = "ITwoFactorService";
        private const string SteamCommunityHost = "steamcommunity.com";
        private const string SteamHelpHost = "help.steampowered.com";
        private const string SteamStoreHost = "store.steampowered.com";

        public SteamHelper(Bot bot, bool disposeWebHandler = false)
        {
            Bot = bot;
            DisposeWebHandler = disposeWebHandler;
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

            var badges = response.Children.First(x => x.Name == "badges").Children
                .Select(x => new Badge(
                    int.Parse(x["badgeid"].Value),
                    int.Parse(x["level"].Value),
                    int.Parse(x["completion_time"].Value),
                    int.Parse(x["xp"].Value),
                    int.Parse(x["scarcity"].Value)
                ));

            var badgesResponse = new BadgesResponse(
                badges.ToList(),
                int.Parse(response["player_xp"].Value),
                int.Parse(response["player_level"].Value),
                int.Parse(response["player_xp_needed_to_level_up"].Value),
                int.Parse(response["player_xp_needed_current_level"].Value)
                );

            // var var
            
            return badgesResponse;
        }
    }
}