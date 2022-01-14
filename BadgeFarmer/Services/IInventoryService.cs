using System.Collections.Generic;
using System.Threading.Tasks;
using BadgeFarmer.Models;

namespace BadgeFarmer.Services;

public interface IInventoryService
{
    Task<IList<AccountCard>> GetAccountCards();
}