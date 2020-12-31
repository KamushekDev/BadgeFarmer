using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BadgeFarmer.Models.Responses
{
    [UsedImplicitly]
    public record BadgesResponse(
        [property: JsonProperty("badges")] IList<Badge> Badges,
        [property: JsonProperty("player_xp")] int PlayerXp,
        [property: JsonProperty("player_level")] int PlayerLevel,
        [property: JsonProperty("player_xp_needed_to_level_up")] int PlayerXpNeededToLevelUp,
        [property: JsonProperty("player_xp_needed_current_level")] int PlayerXpNeededCurrentLevel);
}