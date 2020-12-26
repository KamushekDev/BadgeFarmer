using System;
using System.Composition;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;
using SteamKit2;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand, IBotConnection
    {
        public string Name => "Badge farmer";
        public Version Version => typeof(BadgeFarmer)?.Assembly.GetName().Version ?? new Version(0, 0);

        private SteamHelper? SteamHelper;

        public void OnLoaded()
        {
            Console.WriteLine($"{nameof(BadgeFarmer)} plugin was loaded.");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
        {
            if (SteamHelper != null)
            {
                switch (message[0])
                {
                    case '0':
                        var badges = await SteamHelper.GetBadges();
                        return $"Badges success. Total badges: {badges.Badges.Count}.";
                    case '1':
                        var games = await SteamHelper.GetGames();
                        return $"Games success. Total games: {0}.";
                    case '2':
                        var prices = await SteamHelper.GetPrices();
                        return $"Card prices success. Success: {prices.Success}. Total cards: {prices.TotalCount}.";
                    default:
                        return "Unknown command";
                }
            }

            return "Fault";
        }

        public void OnBotDisconnected(Bot bot, EResult reason)
        {
            SteamHelper = null;
        }

        public void OnBotLoggedOn(Bot bot)
        {
            SteamHelper = new SteamHelper(bot);
        }
    }
}