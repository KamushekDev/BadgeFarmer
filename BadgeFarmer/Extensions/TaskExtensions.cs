using System.Threading.Tasks;

namespace BadgeFarmer.Extensions;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
    }

    public static void FireAndForget<T>(this Task<T> task)
    {
    }
}