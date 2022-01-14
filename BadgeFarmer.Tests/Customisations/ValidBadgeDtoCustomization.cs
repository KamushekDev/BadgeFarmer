using AutoFixture;
using BadgeFarmer.Models;
using BadgeFarmer.Tests.Helpers;
using BadgeFarmer.Tests.Stubs;

namespace BadgeFarmer.Tests.Customisations;

public class ValidBadgeDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<BadgeDto>(x => x.FromFactory(
                (BadgeStub y) => StubCreationHelper.CreateBadgeDto(y)
            )
            .Without(y => y.Level)
            .Without(y => y.Appid)
            .Without(y => y.Xp)
        );
    }
}