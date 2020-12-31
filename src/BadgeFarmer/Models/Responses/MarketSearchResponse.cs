using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BadgeFarmer.Models.Responses
{
    [UsedImplicitly]
    public record MarketSearchResponse(
        [property: JsonProperty("success")] bool Success,
        [property: JsonProperty("start")] int Start,
        [property: JsonProperty("pagesize")] int PageSize,
        [property: JsonProperty("total_count")] int TotalCount,
        [property: JsonProperty("searchdata")] SearchData SearchData,
        [property: JsonProperty("results")] IEnumerable<SearchEntry> SearchEntries
    );

    [UsedImplicitly]
    public record SearchData(
        [property: JsonProperty("query")] string Query,
        [property: JsonProperty("search_descriptions")] bool SearchDescriptions,
        [property: JsonProperty("total_count")] int TotalCount,
        [property: JsonProperty("pagesize")] int PageSize,
        [property: JsonProperty("prefix")] string Prefix,
        [property: JsonProperty("class_prefix")] string ClassPrefix
    );

    [UsedImplicitly]
    public record SearchEntry(
        [property: JsonProperty("name")] string Name,
        [property: JsonProperty("hash_name")] string HashName,
        [property: JsonProperty("sell_listings")] int SellListings,
        [property: JsonProperty("sell_price")] int SellPrice,
        [property: JsonProperty("sell_price_text")] string SellPriceText,
        [property: JsonProperty("app_icon")] string AppIcon,
        [property: JsonProperty("app_name")] string AppName,
        [property: JsonProperty("asset_description")] AssetDescription AssetDescription,
        [property: JsonProperty("sale_price_text")] string SalePriceText
    );

    [UsedImplicitly]
    public record AssetDescription(
        [property: JsonProperty("appid")] int AppId,
        [property: JsonProperty("classid")] string ClassId,
        [property: JsonProperty("instanceid")] string InstanceId,
        [property: JsonProperty("background_color")] string BackgroundColor,
        [property: JsonProperty("icon_url")] string IconURL,
        [property: JsonProperty("tradable")] int Tradable,
        [property: JsonProperty("name")] string Name,
        [property: JsonProperty("name_color")] string NameColor,
        [property: JsonProperty("type")] string Type,
        [property: JsonProperty("market_name")] string MarketName,
        [property: JsonProperty("market_hash_name")] string MarketHashName,
        [property: JsonProperty("commodity")] int Commodity
    );
}