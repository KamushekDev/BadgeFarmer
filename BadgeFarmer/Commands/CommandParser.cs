using System.Linq;
using BadgeFarmer.Commands.Cards;
using BadgeFarmer.Exceptions;

namespace BadgeFarmer.Commands;

public class CommandParser
{
    public ICommand Parse(string command)
    {
        switch (command)
        {
            case "cards update-cache":
            {
                return new UpdateCardsCacheCommand();
            }
            case "cards save":
            {
                return new SaveCardsCacheCommand();
            }
            case "cards load":
            {
                return new LoadCardsCacheCommand();
            }
            case "cards count":
            {
                return new CountCardsInCacheCommand();
            }
            default:
            {
                if (command.StartsWith("badges get crafts "))
                {
                    var args = command.Split(' ')
                        .Skip(3)
                        .Select(x => uint.Parse(x.Trim()))
                        .ToList();

                    var res = new GetBadgeCraftsForMoney
                    {
                        Money = args[0],
                        PriceOverpay = args[1]
                    };
                    return res;
                }

                throw new CommandParseException("xyq");
            }
        }
    }
}