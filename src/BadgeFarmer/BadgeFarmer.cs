using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;
using BadgeFarmer.Extra;
using BadgeFarmer.Models;
using BadgeFarmer.Models.Responses;
using SteamKit2;

namespace BadgeFarmer
{
    [Export(typeof(IPlugin))]
    public class BadgeFarmer : IBotCommand, IBotConnection
    {
        public string Name => "Badge farmer";
        public MarketData? MarketData { get; set; }
        public Version Version => typeof(BadgeFarmer).Assembly.GetName().Version ?? new Version(0, 0);

        private SteamHelper? SteamHelper;

        private bool IsReady()
        {
            return MarketData != null && SteamHelper != null;
        }

        public void OnLoaded()
        {
            Console.WriteLine($"{nameof(BadgeFarmer)} plugin was loaded.");
        }

        public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
        {
            if (!IsReady())
                return "Bot isn't initialized yet.";
            try
            {
                switch (message[0])
                {
                    case '0':
                        var badges = await SteamHelper.GetBadges();
                        return $"Badges success. Total badges: {badges.Badges.Count}.";
                    case '1':
                        var games = await SteamHelper.GetGames();
                        return $"Games success. Total games: {games}.";
                    case '2':
                        var prices = await SteamHelper.QueryMarket();
                        return $"Card prices success. Success: {prices.Success}. Total cards: {prices.TotalCount}.";
                    case '3':
                        var price = await SteamHelper.PriceOverview(753, "336940-Bankers");
                        return $"Price overview success. Success: {price.Success}. Price: {price.LowestPrice}.";
                    case '4':
                        var upperBound = int.Parse(message.Split(' ')[1]);
                        var card = await UpdateCardPrices(upperBound);
                        return
                            $"{card.Count} cards were cached.";
                    case '5':
                        await MarketData!.Save().ConfigureAwait(false);
                        return "Data saved.";
                    case '6':
                        await GetGames();
                        return $"Got games.";
                    case '7':
                        await GetGameBadges();
                        return "Got badges";
                    case '8':
                        await SortBadges();
                        return $"Badges were sorted by their price.";
                    case '9':
                        return await GetLinkForCheapestBadgeCards();
                    case 'u':
                        await UpdateNeededBadges();
                        return "Amount of your badges was updated.";
                    default:
                        return "Unknown command";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "Fault";
        }

        private async Task<IList<SearchEntry>> UpdateCardPrices(int upperBound)
        {
            try
            {
                const int pageSize = 100;

                int l = 0, r = 0, current;
                // temporary optimization (first available cards starts around 13060)
                l = 12930;
                r = 13130;
                MarketSearchResponse cards;
                do
                {
                    current = (l + r) / 2;
                    Console.WriteLine($"Start query with l={l} & r={r} & c={current}");
                    cards = await SteamHelper.QueryMarket(
                        itemClass: "tag_item_class_2",
                        sortColumn: "price",
                        sortDir: "asc",
                        appid: "753",
                        game: "any",
                        start: current,
                        count: pageSize
                    );

                    //todo: отправлять запросы сначала на 1 элемент, и чем меньше разрыв между l и r, тем больше count (до pagesize)

                    if (r == 0)
                        r = cards.TotalCount;

                    var maxPrice = cards.SearchEntries.Max(x => x.SellPrice);
                    var minPrice = cards.SearchEntries.Min(x => x.SellPrice);

                    if (maxPrice > 0 && (minPrice == 0 || current == 0))
                        break;

                    if (maxPrice > 0)
                        r = current;
                    else
                        l = current;
                } while (true);

                var pricedCards = new List<SearchEntry>();

                int currentPrice = 0;
                do
                {
                    Console.WriteLine(
                        $"Finding cards. Current={current}, cards.Count={pricedCards.Count}. CurrentPrice={currentPrice}");
                    var c = await SteamHelper.QueryMarket(
                        itemClass: "tag_item_class_2",
                        sortColumn: "price",
                        sortDir: "asc",
                        appid: "753",
                        game: "any",
                        start: current,
                        count: pageSize
                    );

                    pricedCards.AddRange(c.SearchEntries.Where(x => x.SellListings > 0));

                    current += pageSize;
                    currentPrice = c.SearchEntries.LastOrDefault()?.SellPrice ?? int.MaxValue;
                } while (currentPrice < upperBound * 100);

                MarketData.Cards = pricedCards;
                return MarketData.Cards;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await MarketData.Save().ConfigureAwait(false);
            }
        }

        private async Task GetGames()
        {
            Console.WriteLine($"Starting extract game ids from {MarketData.Cards.Count} cards.");
            var games = new SortedSet<long>();

            foreach (var card in MarketData.Cards)
            {
                var firstSegmentLength = card.HashName.IndexOf('_');
                if (firstSegmentLength == -1)
                    firstSegmentLength = card.HashName.IndexOf('-');
                else if (card.HashName.IndexOf('-') != -1)
                    firstSegmentLength = Math.Min(card.HashName.IndexOf('_'), card.HashName.IndexOf('-'));

                var gameName = card.HashName.Substring(0, firstSegmentLength);
                games.Add(long.Parse(gameName));
            }

            MarketData.GameIds = games;
            await MarketData.Save().ConfigureAwait(false);
            Console.WriteLine($"{games.Count} distinct games were found.");
        }

        private async Task GetGameBadges()
        {
            try
            {
                var badges = new List<BadgeCards>();
                MarketData.BadgeCardsList = badges;
                MarketData.SkippedGameIds.Clear();
                var count = 1;
                var ownedBadges =
                    (await SteamHelper.GetBadges().ConfigureAwait(false)).Badges
                    .Where(x => x.Appid != null)
                    .GroupBy(x => x.Appid!.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var gameId in MarketData.GameIds)
                {
                    try
                    {
                        Console.WriteLine($"Handling {count} game from {MarketData.GameIds.Count}");
                        var gameCards = await SteamHelper.QueryMarket(game: $"tag_app_{gameId}");
                        ownedBadges.TryGetValue(gameId, out var ownedGameBadges);
                        var needed = 5 - (ownedGameBadges?.FirstOrDefault(x => !x.Foil)?.Level ?? 0);
                        var badge = new BadgeCards(gameId, false,
                            gameCards.SearchEntries.Where(x => !x.HashName.Contains("Foil")),
                            needed
                        );
                        needed = 5 - (ownedGameBadges?.FirstOrDefault(x => x.Foil)?.Level ?? 0);
                        var foilBadge = new BadgeCards(gameId, true,
                            gameCards.SearchEntries.Where(x => x.HashName.Contains("Foil")),
                            needed
                        );
                        if (badge.Cards.Any())
                            badges.Add(badge);
                        if (foilBadge.Cards.Any())
                            badges.Add(foilBadge);
                    }
                    catch (Exception e)
                    {
                        MarketData.SkippedGameIds.Add(gameId);
                        Console.WriteLine(e);
                    }

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                await MarketData.Save().ConfigureAwait(false);
            }

            int a = 3;
        }

        private async Task SortBadges()
        {
            try
            {
                MarketData!.BadgeCardsList.Sort(new BadgeCardsComparer(true));
                await MarketData.Save().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<BadgeCards> GetBadgeCards(long gameId, bool foil,
            IDictionary<long, List<Badge>> ownedBadges = null)
        {
            if (ownedBadges == null)
                ownedBadges =
                    (await SteamHelper.GetBadges().ConfigureAwait(false)).Badges
                    .Where(x => x.Appid != null)
                    .GroupBy(x => x.Appid!.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            var gameCards = await SteamHelper.QueryMarket(game: $"tag_app_{gameId}");
            ownedBadges.TryGetValue(gameId, out var ownedGameBadges);
            var needed = 5 - (ownedGameBadges?.FirstOrDefault(x => x.Foil == foil)?.Level ?? 0);
            var badge = new BadgeCards(gameId, foil,
                gameCards.SearchEntries.Where(x => !x.HashName.Contains("Foil")),
                needed
            );
            return badge;
        }

        private async Task<string> GetLinkForCheapestBadgeCards()
        {
            //https://steamcommunity.com/market/multibuy
            //?appid=753
            //&items[]=226840-Warlord&qty[]=1
            //&items[]=226840-Theocrat&qty[]=1
            //&items[]=226840-Rogue&qty[]=1

            var badge = MarketData!.BadgeCardsList.FirstOrDefault(x => x.MaxAtPrice > 0 && x.MaxNeeded > 0);
            badge = await GetBadgeCards(badge!.GameId, badge!.Foil);
            if (badge == null)
                return "There is no available badge or you have not enough data.";
            var url = "https://steamcommunity.com/market/multibuy?appid=753";
            var quantity = Math.Min(badge.MaxNeeded, badge.MaxAtPrice);
            foreach (var card in badge.Cards)
            {
                url += $"&items[]={card.HashName}&qty[]={quantity}";
            }

            return
                $"Badge for gameId={badge.GameId} with approximate pricing at {badge.ApproximatePrice}:{Environment.NewLine}{Uri.EscapeUriString(url)}";
        }

        private async Task UpdateNeededBadges()
        {
            var ownedBadges =
                (await SteamHelper.GetBadges().ConfigureAwait(false)).Badges
                .Where(x => x.Appid != null)
                .GroupBy(x => x.Appid!.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            for (int i = 0; i < MarketData!.BadgeCardsList.Count; i++)
            {
                var badge = MarketData!.BadgeCardsList[i];
                ownedBadges.TryGetValue(badge.GameId, out var ownedGameBadges);
                var needed = 5 - (ownedGameBadges?.FirstOrDefault(x => x.Foil == badge.Foil)?.Level ?? 0);
                if (badge.MaxNeeded != needed)
                    MarketData!.BadgeCardsList[i] = badge with {MaxNeeded = needed};
            }

            await MarketData!.Save().ConfigureAwait(false);
        }

        public async void OnBotDisconnected(Bot bot, EResult reason)
        {
            SteamHelper = null;
            if (MarketData != null)
                await MarketData.Save();
        }

        public async void OnBotLoggedOn(Bot bot)
        {
            SteamHelper = new SteamHelper(bot);
            MarketData = await MarketData.Load().ConfigureAwait(false);
        }
    }
}