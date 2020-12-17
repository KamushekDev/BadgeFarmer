using System.Collections.Generic;

namespace BadgeFarmer.Responses
{
    public record BadgesResponse(IList<Models.Badge> Badges, int PlayerXp, int PlayerLevel, int PlayerXpNeededToLevelUp,
        int PlayerXpNeededCurrentLevel) : IResponse;
}