using System.Collections.Generic;
using System.Xml.XPath;
using BadgeFarmer.Models.Responses;

namespace BadgeFarmer.Extra
{
    public class SearchEntryPriceComparer : IComparer<SearchEntry>
    {
        private readonly int SortOrder;

        public SearchEntryPriceComparer(bool sortAscending)
        {
            SortOrder = sortAscending ? 1 : -1;
        }

        public int Compare(SearchEntry x, SearchEntry y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return SortOrder * 1;
            if (ReferenceEquals(null, x)) return SortOrder * -1;
            return SortOrder * x.SellPrice.CompareTo(y.SellPrice);
        }
    }
}