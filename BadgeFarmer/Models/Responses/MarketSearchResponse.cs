using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BadgeFarmer.Models.Responses
{
    public record MarketSearchResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("start")] int Start,
        [property: JsonPropertyName("pagesize")] int PageSize,
        [property: JsonPropertyName("total_count")] int TotalCount,
        [property: JsonPropertyName("searchdata")] SearchData SearchData,
        [property: JsonPropertyName("results")] IEnumerable<SearchEntry> SearchEntries
    );

    public record SearchData(
        [property: JsonPropertyName("query")] string Query,
        [property: JsonPropertyName("search_descriptions")] bool SearchDescriptions,
        [property: JsonPropertyName("total_count")] int TotalCount,
        [property: JsonPropertyName("pagesize")] int PageSize,
        [property: JsonPropertyName("prefix")] string Prefix,
        [property: JsonPropertyName("class_prefix")] string ClassPrefix
    );

    public record SearchEntry(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("hash_name")] string HashName,
        [property: JsonPropertyName("sell_listings")] int SellListings,
        [property: JsonPropertyName("sell_price")] int SellPrice,
        [property: JsonPropertyName("sell_price_text")] string SellPriceText,
        [property: JsonPropertyName("app_icon")] string AppIcon,
        [property: JsonPropertyName("app_name")] string AppName,
        [property: JsonPropertyName("asset_description")] AssetDescription AssetDescription,
        [property: JsonPropertyName("sale_price_text")] string SalePriceText
    );

    public record AssetDescription(
        [property: JsonPropertyName("appid")] int AppId,
        [property: JsonPropertyName("classid")] string ClassId,
        [property: JsonPropertyName("instanceid")] string InstanceId,
        [property: JsonPropertyName("background_color")] string BackgroundColor,
        [property: JsonPropertyName("icon_url")] string IconURL,
        [property: JsonPropertyName("tradable")] int Tradable,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("name_color")] string NameColor,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("market_name")] string MarketName,
        [property: JsonPropertyName("market_hash_name")] string MarketHashName,
        [property: JsonPropertyName("commodity")] int Commodity
    );
}