using Newtonsoft.Json;

namespace BadgeFarmer
{
    
    // {
    //     "success": true,
    //     "lowest_price": "3,68 pуб.",
    //     "volume": "30,400",
    //     "median_price": "4,70 pуб."
    // }
    
    public class ItemPrice
    {
        [JsonProperty("success")]
        public bool Success { get; }

        [JsonProperty("lowest_price")]
        //[JsonConverter(typeof(StringToDecimalConverter))]
        public string LowerPrice { get; }

        [JsonProperty("volume")]
        public string Volume { get; }

        [JsonProperty("median_price")]
        //[JsonConverter(typeof(StringToDecimalConverter))]
        public string MedianPrice { get; }

        public ItemPrice(bool success, string lowerPrice, string volume, string medianPrice)
        {
            Success = success;
            LowerPrice = lowerPrice;
            Volume = volume;
            MedianPrice = medianPrice;
        }
    }
}