namespace BadgeFarmer.Commands.Cards;

public class GetBadgeCraftsForMoney : ICommand
{
    public uint Money { get; set; }
    public uint PriceOverpay { get; set; }
}