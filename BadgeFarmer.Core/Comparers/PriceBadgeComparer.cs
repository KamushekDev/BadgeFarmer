using BadgeFarmer.Core.Models;

namespace BadgeFarmer.Core.Comparers;

public class PriceBadgeComparer : IComparer<Badge>
{
    public int Compare(Badge x, Badge y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        var minimalPriceComparison = x.MinimalPrice.CompareTo(y.MinimalPrice);
        if (minimalPriceComparison != 0) return minimalPriceComparison;
        var appIdComparison = x.AppId.CompareTo(y.AppId);
        if (appIdComparison != 0) return appIdComparison;
        return x.IsFoil.CompareTo(y.IsFoil);
    }
}