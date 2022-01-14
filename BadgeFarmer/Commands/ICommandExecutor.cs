using System.Threading.Tasks;

namespace BadgeFarmer.Commands;

public interface ICommandExecutor
{
    Task<string> Execute(ICommand command);
}