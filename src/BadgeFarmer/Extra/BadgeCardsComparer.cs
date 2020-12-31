using System.Collections.Generic;
using BadgeFarmer.Models;

namespace BadgeFarmer.Extra
{
    public class BadgeCardsComparer : IComparer<BadgeCards>
    {
        private readonly int SortOrder;

        public BadgeCardsComparer(bool sortAscending)
        {
            SortOrder = sortAscending ? 1 : -1;
        }

        public int Compare(BadgeCards? x, BadgeCards? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return SortOrder * 1;
            if (ReferenceEquals(null, x)) return SortOrder * -1;
            if (x.MaxNeeded == 0 && y.MaxNeeded == 0)
                return SortOrder * x.ApproximatePrice.CompareTo(y.ApproximatePrice);
            if (x.MaxNeeded == 0)
                return SortOrder;
            if (y.MaxNeeded == 0)
                return SortOrder * -1;
            return SortOrder * x.ApproximatePrice.CompareTo(y.ApproximatePrice);
        }
    }
}