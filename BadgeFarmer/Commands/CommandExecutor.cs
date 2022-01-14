using System;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm;
using BadgeFarmer.Commands.Cards;
using BadgeFarmer.Extensions;
using BadgeFarmer.Services;

namespace BadgeFarmer.Commands;

//todo: handlers might be better
public class CommandExecutor : ICommandExecutor
{
    private readonly ICardsService _cardsService;
    private readonly IInventoryService _inventoryService;
    private readonly IBadgesService _badgesService;

    public CommandExecutor(ICardsService cardsService, IInventoryService inventoryService, IBadgesService badgesService)
    {
        _cardsService = cardsService;
        _inventoryService = inventoryService;
        _badgesService = badgesService;
    }

    public async Task<string> Execute(ICommand command)
    {
        switch (command)
        {
            case UpdateCardsCacheCommand:
            {
                var progress = new Progress<(int current, int total)>(p =>
                {
                    ASF.ArchiLogger.LogGenericInfo(
                        $"Updating cache: {p.current} out of {p.total}.");
                });
                _cardsService.UpdateCache(progress).FireAndForget();
                return "Cache update is started...";
            }
            case SaveCardsCacheCommand:
            {
                await _cardsService.SaveCache();
                return "Cache was saved.";
            }
            case LoadCardsCacheCommand:
            {
                await _cardsService.LoadCache();
                return "Cache was loaded.";
            }
            case CountCardsInCacheCommand:
            {
                return $"Service have info about {_cardsService.CardsTotal} cards";
            }
            case GetBadgeCraftsForMoney cmd:
            {
                var crafts = await _badgesService.GetBadgeCraftsForMoney(cmd.Money, cmd.PriceOverpay);
                var totalPrice = crafts.Sum(x => x.Cards.Sum(y => y.SellPrice));
                var links = _badgesService.GetMultibuyLinks(crafts);
                return
                    $"Got {crafts.Count} badges for price of {totalPrice} cents.{Environment.NewLine}{string.Join(Environment.NewLine, links)}";
            }
            default:
                throw new NotImplementedException();
        }
    }
}
//             switch (command)
//             {
//                 case "cards update-cache":
//                 {
//                 }
//                 case "cards save":
//                 {
//
//                 }
//                 case "cards load":
//                 {

//                 }
//                 case "cards count":
//                 {
//
//                 }
//                 case "badges get all":
//                 {
//                     var badges = BadgesService.GetAllBadges();
//                     return $"Got {badges.Count} badges of {badges.Sum(x => x.MinimalPrice) / 100} rub.";
//                 }
//                 case "badges get available":
//                 {
//                     var badges = await BadgesService.GetAvailableForAccountBadges();
//                     return
//                         $"Got {badges.Count} badges of {badges.Sum(x => x.badge.MinimalPrice * x.needed) / 100} rub.";
//                 }
//                 case "inventory get cards":
//                 {
//                     var accountCards = await InventoryService.GetAccountCards();
//                     return $"Got {accountCards.Sum(x => x.Amount)} cards.";
//                 }
//                 case "account badges 500":
//                 {
//                     var crafts = await BadgesService.GetBadgesForMoney(500 * 100);
//                     var totalPrice = crafts.Sum(x => x.Cards.Sum(y => y.SellPrice));
//                     return $"Got {crafts.Count} badges for price of {totalPrice / 100 + 1} rub.";
//                 }
//                 default:
//                     return "Unknown command";
//             }