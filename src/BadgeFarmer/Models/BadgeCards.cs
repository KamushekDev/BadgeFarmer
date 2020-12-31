using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BadgeFarmer.Models.Responses;
using Newtonsoft.Json;

namespace BadgeFarmer.Models
{
    public record BadgeCards(
        [property: JsonProperty("gameId")] long GameId,
        [property: JsonProperty("foil")] bool Foil,
        [property: JsonProperty("cards")] IEnumerable<SearchEntry> Cards,
        [property: JsonProperty("maxNeeded")] int MaxNeeded = 5
    )
    {
        public decimal ApproximatePrice => Cards.Sum(x => x.SellPrice) / 100.0m;
        public int MaxAtPrice => Cards.Min(x => x.SellListings);
    }
}