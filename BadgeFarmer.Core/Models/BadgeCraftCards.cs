namespace BadgeFarmer.Core.Models;

public class BadgeCraftCards
{
    public Badge Badge { get; init; }
    public IReadOnlyCollection<Card> Cards { get; init; }
}