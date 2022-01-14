using AutoFixture;
using BadgeFarmer.Tests.Helpers;
using BadgeFarmer.Tests.Stubs;
using SteamUtils.Models;

namespace BadgeFarmer.Tests.Customisations;

public class ValidSearchItemCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<SearchMarketResponse.SearchItem>(
            x => x.FromFactory(
                    (CardStub y) => StubCreationHelper.CreateSearchItem(y)
                )
                .OmitAutoProperties());
    }
}