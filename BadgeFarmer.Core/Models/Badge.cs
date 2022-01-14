namespace BadgeFarmer.Core.Models;

public class Badge
{
    public IReadOnlyList<Card> Cards { get; }

    public int AppId { get; }
    public bool IsFoil { get; }
    public int MinimalPrice { get; }
    public int AvailableCount { get; }

    public Badge(IEnumerable<Card> cards)
    {
        Cards = new List<Card>(cards);
        AppId = Cards.First().AppId;
        IsFoil = Cards.First().IsFoil;
        MinimalPrice = Cards.Sum(x => x.SellPrice);
        AvailableCount = Cards.Min(x => x.SellListings);
    }
}