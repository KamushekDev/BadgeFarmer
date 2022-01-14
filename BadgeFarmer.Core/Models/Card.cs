namespace BadgeFarmer.Core.Models;

public class Card
{
    public string Name { get; set; }
    public string HashName { get; set; }
    public string MarketHashName { get; set; }
    public int SellListings { get; set; }
    public int SellPrice { get; set; }
    public string AppName { get; set; }
    public int AppId { get; set; }

    public bool IsFoil => HashName.Contains("Foil", StringComparison.InvariantCultureIgnoreCase);
}