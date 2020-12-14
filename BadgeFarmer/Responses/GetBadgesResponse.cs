using System.Collections.Generic;

namespace BadgeFarmer.Responses
{
    public record GetBadgesResponse(IList<Badge> Badges, int PlayerXp, int PlayerLevel, int PlayerXpNeededToLevelUp,
        int PlayerXpNeededCurrentLevel) : IResponse;
}