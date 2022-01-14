using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Common;
using BadgeFarmer.Clients;
using BadgeFarmer.Core.Comparers;
using BadgeFarmer.Core.Models;

namespace BadgeFarmer.Services;

public class BadgesService : IBadgesService
{
    private readonly ICardsService _cardsService;
    private readonly IInventoryService _inventoryService;
    private readonly ICustomSteamClient _client;

    public BadgesService(ICustomSteamClient client, ICardsService cardsService, IInventoryService inventoryService)
    {
        _cardsService = cardsService;
        _inventoryService = inventoryService;
        _client = client;
    }

    public IList<Badge> GetAllBadges()
    {
        var badges = _cardsService.Cards
            .GroupBy(x => x.AppId)
            .Select(x => x
                .GroupBy(y => y.IsFoil)
                .Select(y => new Badge(y)))
            .SelectMany(x => x);

        return badges.ToList();
    }

    public async Task<ICollection<(Badge badge, int needed)>> GetAvailableForAccountBadges()
    {
        var badges = GetAllBadges();
        var badgesResponse = await _client.GetBadges();
        var badgesOnAccount =
            badgesResponse.Badges
                .Where(x => !string.IsNullOrEmpty(x.Appid))
                .ToDictionary(x => int.Parse(x.Appid));
        var result = new List<(Badge, int)>();

        foreach (var badge in badges)
        {
            if (badgesOnAccount.TryGetValue(badge.AppId, out var claimedBadge))
            {
                result.Add((badge, 5 - claimedBadge.Level));
            }
            else
                result.Add((badge, 5));
        }

        return result.Where(x => x.Item2 > 0).ToList();
    }

    public async Task<IList<BadgeCraftCards>> GetBadgeCraftsForMoney(uint money, uint overpay)
    {
        var badges = await GetAvailableForAccountBadges();
        var cards = await _inventoryService.GetAccountCards();
        var cardAmounts = cards.ToDictionary(x => x.MarketHashName, x => x.Amount);

        var crafts = new List<BadgeCraftCards>();
        foreach (var badge in badges)
        {
            var neededCards = badge.badge.Cards;
            for (int i = 0; i < badge.needed; i++)
            {
                var cardsForCraft = new List<Card>();
                foreach (var card in neededCards)
                {
                    if (cardAmounts.TryGetValue(card.MarketHashName, out var amount) && amount > 0)
                    {
                        cardAmounts[card.MarketHashName]--;
                        continue;
                    }

                    cardsForCraft.Add(card);
                }

                crafts.Add(new BadgeCraftCards { Cards = cardsForCraft.AsReadOnly(), Badge = badge.badge });
            }
        }

        crafts = crafts.Where(x => x.Cards.All(y => y.SellListings > 0))
            .OrderBy(x => x.Cards.Sum(y => y.SellPrice))
            .ToList();

        var result = new List<BadgeCraftCards>();

        var maxMoney = money * 100 / (100 + overpay);
        var currentMoney = 0;
        var craftIndex = 0;

        while (true)
        {
            var currentSum = crafts[craftIndex].Cards.Sum(x => x.SellPrice);
            if (currentMoney + currentSum < maxMoney)
            {
                currentMoney += currentSum;
                result.Add(crafts[craftIndex++]);

                if (craftIndex >= crafts.Count)
                    break;
            }
            else
                break;
        }

        return result;
    }

    public IList<string> GetMultibuyLinks(ICollection<BadgeCraftCards> crafts)
    {
        var neededCards = crafts.SelectMany(x => x.Cards)
            .GroupBy(x => x.MarketHashName, _ => 1, (name, counts) => (name, counts.Sum()));

        var links = neededCards.Chunk(15).Select(chunk =>
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("https://steamcommunity.com/market/multibuy?appid=753");
            foreach (var card in chunk)
            {
                queryBuilder.AppendFormat("&items[]={0}&qty[]={1}", card.name, card.Item2);
            }

            return queryBuilder.ToString();
        });

        return links.ToList();
    }
}