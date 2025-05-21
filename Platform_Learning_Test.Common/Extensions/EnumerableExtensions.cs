using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Platform_Learning_Test.Common.Extensions;

public enum ProcessingIntensity
{
    Critical = 2,
    High = 4,
    Normal = 8,
    Low = 12,
    Background = 16
}

public static class EnumerableExtensions
{
    [ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    [ContractAnnotation("null => false")]
    public static bool HasItems<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any();
    }

    public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static async Task ProcessInParallelAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task> processor,
        ProcessingIntensity intensity = ProcessingIntensity.Normal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(processor);

        using var semaphore = new SemaphoreSlim((int)intensity, (int)intensity);
        var tasks = source.Select(async item =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await processor(item).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public static async Task<TOut[]> ProcessInParallelAsync<TIn, TOut>(
        this IEnumerable<TIn> source,
        Func<TIn, Task<TOut>> processor,
        ProcessingIntensity intensity = ProcessingIntensity.Normal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(processor);

        using var semaphore = new SemaphoreSlim((int)intensity, (int)intensity);
        var tasks = source.Select(async item =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await processor(item).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}