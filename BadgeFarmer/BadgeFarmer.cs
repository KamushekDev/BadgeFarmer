using System;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Plugins;
using BadgeFarmer.Clients;
using BadgeFarmer.Services;
using SteamKit2;
using SteamUtils.Models;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand, IBotConnection
    {
        public string Name => "Badge farmer";
        public Version Version => typeof(BadgeFarmer)?.Assembly.GetName().Version ?? new Version(0, 1);

        private CustomSteamClient SteamClient;
        private ICardsService CardsService;

        public void OnLoaded()
        {
            Console.WriteLine($"{nameof(BadgeFarmer)} plugin was loaded.");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
        {
            try
            {
                if (SteamClient != null)
                {
                    var commands = message.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    var first = commands.First();
                    if (first == "cards")
                    {
                        var second = commands.Skip(1).First();
                        if (second == "update-cache")
                        {
                            var progress = new Progress<(int current, int total)>(p =>
                            {
                                ASF.ArchiLogger.LogGenericInfo($"Updating cache: {p.current} out of {p.total}.");
                            });
                            CardsService.UpdateCache(progress);
                            return "Cache update is started...";
                        }
                        else if (second == "save")
                        {
                            await CardsService.SaveCache();
                            return "Cache was saved.";
                        }
                        else if (second == "load")
                        {
                            await CardsService.LoadCache();
                            return "Cache was loaded.";
                        }
                        else if (second == "count")
                        {
                            return $"Service have info about {CardsService.CardsTotal} cards";
                        }
                    }
                    else if (first == "proxy")
                    {
                        var second = commands.Skip(1).First();
                        if (second == "update")
                        {
                            var proxies =
                                await File.ReadAllLinesAsync(Path.Combine(SharedInfo.ConfigDirectory, "Proxies.txt"));
                        }
                    }

                    return "Unknown command";

                    // switch (message[0])
                    // {
                    //     case '0':
                    //         var badges = await SteamClient.GetBadges();
                    //         return $"Badges success. Total badges: {badges.Badges.Count}.";
                    //     case '1':
                    //         var games = await SteamClient.GetGames();
                    //         return $"Games success. Total games: {games}.";
                    //     case '2':
                    //         var prices = await SteamClient.GetPrices(new SearchMarketRequest());
                    //         return $"Card prices success. Success: {prices.Success}. Total cards: {prices.TotalCount}.";
                    //     default:
                    // }
                }

                return "Steam client is null :c";
            }
            catch (Exception e)
            {
                return $"Exception was thrown: {e.Message}";
            }
        }

        public void OnBotDisconnected(Bot bot, EResult reason)
        {
            SteamClient = null;
        }

        public void OnBotLoggedOn(Bot bot)
        {
            SteamClient = new CustomSteamClient(bot);
            CardsService = new CardsService(SteamClient, ArchiSteamFarm.SharedInfo.ConfigDirectory);
        }
    }
}