using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiSteamFarm;
using BadgeFarmer.Responses;
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

        internal async Task<GetBadgesResponse> GetBadges()
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

            throw null;
        }
    }
}