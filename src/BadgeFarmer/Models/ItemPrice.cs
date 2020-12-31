using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BadgeFarmer.Models
{
    // {
    //     "success": true,
    //     "lowest_price": "3,68 pуб.",
    //     "volume": "30,400",
    //     "median_price": "4,70 pуб."
    // }
    [UsedImplicitly]
    public record ItemPrice(
        [property: JsonProperty("success")] bool Success,
        [property: JsonProperty("lowest_price")] string LowestPrice,
        [property: JsonProperty("volume")] string Volume,
        [property: JsonProperty("median_price")] string MedianPrice
    );
}