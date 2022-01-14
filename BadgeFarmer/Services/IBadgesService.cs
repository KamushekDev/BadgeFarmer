using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BadgeFarmer.Core.Models;

namespace BadgeFarmer.Services;

public interface IBadgesService
{
    IList<Badge> GetAllBadges();
    Task<ICollection<(Badge badge, int needed)>> GetAvailableForAccountBadges();
    Task<IList<BadgeCraftCards>> GetBadgeCraftsForMoney(uint money, uint overpay);
    IList<string> GetMultibuyLinks(ICollection<BadgeCraftCards> crafts);
}