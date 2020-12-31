using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BadgeFarmer.Models
{
    [UsedImplicitly]
    public record Badge(
        [property: JsonProperty("badgeid")] int BadgeId,
        [property: JsonProperty("level")] int Level,
        [property: JsonProperty("completion_time")] int CompletionTime,
        [property: JsonProperty("xp")] int Xp,
        [property: JsonProperty("scarcity")] int Scarcity,
        [property: JsonProperty("appid")] long? Appid,
        [property: JsonProperty("communityitemid")] string? CommunityItemId,
        [property: JsonProperty("border_color")] int? BorderColor
    )
    {
        public bool Foil => BorderColor == 1;
    }
}