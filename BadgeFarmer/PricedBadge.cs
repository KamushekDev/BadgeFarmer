using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SteamKit2;

namespace BadgeFarmer
{
    public class PricedBadge
    {
        public BadgeOLD BadgeOld { get; }

        public decimal Price { get; }

        public ECurrencyCode Currency { get; }

        public PricedBadge(BadgeOLD badgeOld, IList<ItemPrice> prices, ECurrencyCode currency)
        {
            BadgeOld = badgeOld;
            Currency = currency;

            decimal price = 0;//prices.Sum(x => x.LowerPrice);

            Price = price;
        }
    }
}