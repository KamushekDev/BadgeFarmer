using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using BadgeFarmer.Clients;
using BadgeFarmer.Core.Models;
using BadgeFarmer.Models;
using BadgeFarmer.Models.Responses;
using BadgeFarmer.Services;
using BadgeFarmer.Tests.Helpers;
using BadgeFarmer.Tests.Stubs;
using DeepCopy;
using FluentAssertions;
using Moq;
using Xunit;

namespace BadgeFarmer.Tests;

public class BadgesServiceTests
{
    private readonly BadgesService _service;
    private readonly Mock<IInventoryService> _inventoryServiceMock;
    private readonly Mock<ICustomSteamClient> _customSteamClient;
    private readonly Mock<ICardsService> _cardsService;
    private readonly Fixture _fixture;

    public BadgesServiceTests()
    {
        _inventoryServiceMock = new Mock<IInventoryService>(MockBehavior.Strict);
        _customSteamClient = new Mock<ICustomSteamClient>(MockBehavior.Strict);
        _cardsService = new Mock<ICardsService>(MockBehavior.Strict);
        _service = new BadgesService(_customSteamClient.Object, _cardsService.Object, _inventoryServiceMock.Object);
        _fixture = new Fixture();
    }

    [Theory]
    [AutoData]
    public async Task GroupBadges_ValidBadges_Test(List<Card> cards)
    {
        // ARRANGE
        SetupFixture();
        SetupCardsService(cards);

        // ACT
        var badges = _service.GetAllBadges();

        // ASSERT
        badges.Should().OnlyContain(bool(x) => CheckBadge(x));
    }

    private bool CheckBadge(Badge badge)
    {
        return badge.Cards.All(y => y.IsFoil == badge.IsFoil && y.AppId == badge.AppId)
               && badge.MinimalPrice == badge.Cards.Sum(y => y.SellPrice)
               && badge.AvailableCount == badge.Cards.Min(y => y.SellListings);
    }


    [Theory]
    [AutoData]
    public async Task GroupBadges_SortedCorrectly_Test(List<Card> cards, int money, int overpay)
    {
        // ARRANGE
        SetupFixture();
        SetupCardsService(cards);
        SetupInventoryService(new List<AccountCard>());
        SetupCustomSteamClient(new List<BadgeStub>());

        // ACT
        var badgeCrafts = await _service.GetBadgeCraftsForMoney((uint)money, (uint)overpay);

        // ASSERT
        badgeCrafts.Sum(x => x.Cards.Sum(y => y.SellPrice)).Should().BeLessOrEqualTo(money * 100 / (100 + overpay));
    }

    [AutoData]
    [Theory(DisplayName = "Значки с одинаковой ценой не игнорируются")]
    public async Task GroupBadgesEqualPrices_Test(Card card1, Card card2)
    {
        // ARRANGE
        SetupFixture();
        card1.SellPrice = card2.SellPrice;
        SetupCardsService(new[] { card1, card2 });

        // ACT
        var badges = _service.GetAllBadges();

        // ASSERT
        badges.Should().HaveCount(2);
    }

    private void SetupCardsService(IReadOnlyCollection<Card> cards)
    {
        _cardsService.Setup(x => x.Cards)
            .Returns(cards);
        _cardsService.Setup(x => x.CardsTotal)
            .Returns(cards.Count);
    }

    private void SetupInventoryService(List<AccountCard> cards)
    {
        _inventoryServiceMock.Setup(x => x.GetAccountCards())
            .ReturnsAsync(cards);
    }

    private void SetupCustomSteamClient(List<BadgeStub> stubs)
    {
        var badgesResponse = _fixture.Create<BadgesResponse>();

        var badges = stubs.Select(StubCreationHelper.CreateBadgeDto).ToList();

        badgesResponse = badgesResponse with { Badges = badges };
        _customSteamClient.Setup(x => x.GetBadges())
            .ReturnsAsync(badgesResponse);
    }

    private void SetupFixture()
    {
    }
}