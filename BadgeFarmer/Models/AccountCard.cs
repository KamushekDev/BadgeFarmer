using System;
using ArchiSteamFarm.Json;

namespace BadgeFarmer.Models;

public class AccountCard
{
    public string MarketHashName { get; init; }
    public uint Amount { get; init; }

    public AccountCard(Steam.Asset asset)
    {
        Amount = asset.Amount;
        var marketHashName = asset.AdditionalPropertiesReadOnly?["market_hash_name"].ToString();
        if (marketHashName is not null)
            MarketHashName = marketHashName;
    }

    private AccountCard(string marketHashName, uint amount)
    {
        MarketHashName = marketHashName;
        Amount = amount;
    }

    public static AccountCard Merge(AccountCard a, AccountCard b)
    {
        if (a.MarketHashName != b.MarketHashName)
            throw new ArgumentException("Market hash name of two cards are different.");

        var result = new AccountCard(a.MarketHashName, a.Amount + b.Amount);
        return result;
    }
}