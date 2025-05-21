using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Platform_Learning_Test.Common.Extensions;

public static class TaskExtensions
{
    public static async void FireAndForgetAsync(
        this Task task,
        ILogger? logger = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "")
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "FireAndForget failed in {Caller} ({File})", caller, file);

#if DEBUG
            throw;
#endif
        }
    }

    public static async Task<T?> TryExecuteAsync<T>(
        this Task<T> task,
        ILogger? logger = null,
        T? defaultValue = default,
        [CallerMemberName] string caller = "")
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Operation failed in {Caller}, returning default", caller);
            return defaultValue;
        }
    }
}