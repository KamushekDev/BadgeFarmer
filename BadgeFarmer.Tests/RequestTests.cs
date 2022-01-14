using AutoFixture;
using FluentAssertions;
using SteamUtils.Models;
using Xunit;

namespace BadgeFarmer.Tests;

public class RequestTests
{
    private readonly Fixture _fixture;

    public RequestTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void GetPricesRequestQueryTest()
    {
        // ARRANGE 
        var req = _fixture.Create<SearchMarketRequest>();

        var expected =
            $"/market/search/render/?q={req.Query}&category_753_Game%5B%5D={req.Game}" +
            $"&category_753_item_class%5B%5D={req.ItemClass}&appid={req.AppId}" +
            $"&start={req.Start}&count={req.Count}&sort_column={req.SortColumn}" +
            $"&sort_dir={req.SortDirection}&norender=1";

        // ACT
        var query = req.GetQueryParams();

        // ASSERT
        query.Should().Be(expected);
    }
}