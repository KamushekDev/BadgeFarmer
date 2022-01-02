using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BadgeFarmer.Extensions;

public static class EnumerableExtensions
{
    public static async Task<IReadOnlyList<TOut>> ConcurrentSelectAsync<TIn, TOut>(
        this IEnumerable<TIn> values,
        Func<TIn, CancellationToken, Task<TOut>> selector,
        int concurrencyLevel,
        CancellationToken token)
    {
        var activeTasks = new HashSet<Task<TOut>>(concurrencyLevel, ReferenceEqualityComparer.Instance);
        using var enumerator = values.GetEnumerator();
        for (var i = 0; i < concurrencyLevel && enumerator.MoveNext(); i++)
            activeTasks.Add(selector(enumerator.Current, token));

        var results = new List<TOut>();

        while (activeTasks.Any())
        {
            token.ThrowIfCancellationRequested();
            var finishedTask = await Task.WhenAny(activeTasks);
            activeTasks.Remove(finishedTask);
            results.Add(await finishedTask);

            if (enumerator.MoveNext())
                activeTasks.Add(selector(enumerator.Current, token));
        }

        return results;
    }
}