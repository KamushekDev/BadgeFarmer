using System.Text.Json.Serialization;

namespace BadgeFarmer.Models
{
    public record BadgeDto(
        [property: JsonPropertyName("badgeid")] int BadgeId,
        [property: JsonPropertyName("level")] int Level,
        [property: JsonPropertyName("completion_time")] int CompletionTime,
        [property: JsonPropertyName("xp")] int Xp,
        [property: JsonPropertyName("scarcity")] int Scarcity,
        [property: JsonPropertyName("appid")] string Appid,
        [property: JsonPropertyName("communityitemid")] string? CommunityItemId,
        [property: JsonPropertyName("border_color")] int? BorderColor
    );
}