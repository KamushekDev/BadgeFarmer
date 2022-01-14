using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BadgeFarmer.Clients;
using BadgeFarmer.Services;
using BadgeFarmer.Tests.Customisations;
using Moq;
using SteamUtils.Models;
using Xunit;

namespace BadgeFarmer.Tests;

public class CardsServiceTests
{
    private readonly Services.CardsService _service;
    private readonly Mock<IFileSaver> _fileSaverMock;
    private readonly Mock<ICustomSteamClient> _customSteamClient;

    private readonly Fixture _fixture;

    public CardsServiceTests()
    {
        _fileSaverMock = new Mock<IFileSaver>(MockBehavior.Strict);
        _customSteamClient = new Mock<ICustomSteamClient>(MockBehavior.Strict);
        _service = new Services.CardsService(_customSteamClient.Object, _fileSaverMock.Object);
        _fixture = new Fixture();

        SetupFixture();
    }

    [Fact]
    public async Task CanLoadCards()
    {
        // ARRANGE
        var items = _fixture.CreateMany<SearchMarketResponse.SearchItem>(1).ToList();
        SetupFileSaver(items);

        // ACT
        await _service.LoadCache();

        // ASSERT
        Assert.NotEmpty(_service.Cards);
        Assert.NotEqual(0, _service.CardsTotal);
    }

    [Fact]
    public async Task LoadCards_ParseCorrectly()
    {
        // ARRANGE
        var items = _fixture.CreateMany<SearchMarketResponse.SearchItem>(1).ToList();
        SetupFileSaver(items);
        var item = items.First();

        // ACT
        await _service.LoadCache();
        var card = _service.Cards.First();

        // ASSERT
        Assert.Equal(item.Name, card.Name);
        Assert.Equal(item.SellListings, card.SellListings);
        Assert.Equal(item.SellPrice, card.SellPrice);
        Assert.StartsWith(card.AppId.ToString(),item.AssetDescription.MarketHashName);
        Assert.Equal(item.AssetDescription.MarketHashName, card.MarketHashName);
    }

    private void SetupFileSaver(List<SearchMarketResponse.SearchItem> items)
    {
        _fileSaverMock
            .Setup(x => x.LoadAsync<List<SearchMarketResponse.SearchItem>>(It.IsAny<string>()))
            .ReturnsAsync(items);
    }

    private void SetupFixture()
    {
        _fixture.Customize(new ValidSearchItemCustomization());
    }
}