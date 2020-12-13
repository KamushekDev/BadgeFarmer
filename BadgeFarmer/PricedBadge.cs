using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SteamKit2;

namespace BadgeFarmer
{
    public class PricedBadge
    {
        public Badge Badge { get; }

        public decimal Price { get; }

        public ECurrencyCode Currency { get; }

        public PricedBadge(Badge badge, IList<ItemPrice> prices, ECurrencyCode currency)
        {
            Badge = badge;
            Currency = currency;

            decimal price = 0;//prices.Sum(x => x.LowerPrice);

            Price = price;
        }
    }
}