using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BadgeFarmer.Models.Responses
{
    public record BadgesResponse(
        [property: JsonPropertyName("badges")] IList<BadgeDto> Badges,
        [property: JsonPropertyName("player_xp")] int PlayerXp,
        [property: JsonPropertyName("player_level")] int PlayerLevel,
        [property: JsonPropertyName("player_xp_needed_to_level_up")] int PlayerXpNeededToLevelUp,
        [property: JsonPropertyName("player_xp_needed_current_level")] int PlayerXpNeededCurrentLevel);
}