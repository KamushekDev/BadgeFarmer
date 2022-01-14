using AutoFixture;
using BadgeFarmer.Models;
using BadgeFarmer.Tests.Stubs;
using SteamUtils.Models;

namespace BadgeFarmer.Tests.Helpers;

public static class StubCreationHelper
{
    public static SearchMarketResponse.SearchItem CreateSearchItem(CardStub stub)
    {
        return new SearchMarketResponse.SearchItem(
            stub.Name,
            $"{stub.AppId}-{stub.Name}",
            stub.Amount,
            stub.Price,
            $"${stub.Price / 100}.{stub.Price % 100}",
            "https://cdn.cloudflare.steamstatic.com/steamcommunity/public/images/apps/753/135dc1ac1cd9763dfc8ad52f4e880d2ac058a36c.jpg",
            "Steam",
            new SearchMarketResponse.AssetDescription(
                753,
                "3562033529",
                "0",
                "",
                "IzMF03bk9WpSBq-S-ekoE33L-iLqGFHVaU25ZzQNQcXdA3g5gMEPvUZZEfSMJ6dESN8p_2SVTY7V2NEJxHsKmChCIzb02ClBZPZ4c_nPxAC9qO-MG3GqbGWRKXKASFs_SbFcNm-P-DOj5ujCEGnBEr4rFQkMK6BV9mVAPsvfagx9itAdqWqqk0FvIR8lc8JDLV3qmSRLN7x2kSAWI88EmSDxI5XZhQtgbxRsWOniV7-TbYKjxX4kW0x5X_5Ncs2YuRz0Rzi9",
                1,
                stub.Name,
                "",
                "Suicide Guy: Sleepin' Deeply Foil Trading Card",
                stub.Name,
                $"{stub.AppId}-{stub.Name}",
                1
            ),
            $"${stub.Price / 100}.{stub.Price % 100}"
        );
    }

    public static BadgeDto CreateBadgeDto(BadgeStub stub)
    {
        return new BadgeDto(
            1,
            stub.Level,
            424234,
            stub.Level * 100,
            1,
            stub.AppId.ToString(),
            "",
            null
        );
    }
}