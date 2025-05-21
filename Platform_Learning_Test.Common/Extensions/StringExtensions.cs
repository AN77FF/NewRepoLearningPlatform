using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Platform_Learning_Test.Common.Extensions;

public static class StringExtensions
{
    [ContractAnnotation("null => true")]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    [ContractAnnotation("null => true")]
    public static string? AsNullIfEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static string EnsureNotNull(this string? value)
    {
        return value ?? string.Empty;
    }

    public static bool EqualsOrdinal(this string? source, string? other)
    {
        return string.Equals(source, other, StringComparison.Ordinal);
    }

    public static bool EqualsIgnoreCase(this string? source, string? other)
    {
        return string.Equals(source, other, StringComparison.OrdinalIgnoreCase);
    }

    public static IReadOnlyList<T> ParseCsv<T>(
        this string csv,
        char separator = ',',
        TryParseDelegate<T>? tryParse = null)
    {
        if (string.IsNullOrWhiteSpace(csv))
            return Array.Empty<T>();

        tryParse ??= DefaultTryParse<T>;

        return csv.Split(separator)
                .Select(s => s.Trim())
                .Where(s => !s.IsNullOrWhiteSpace())
                .ParseSafe(tryParse)
                .ToList();
    }

    private static IEnumerable<T> ParseSafe<T>(
        this IEnumerable<string> source,
        TryParseDelegate<T> tryParse)
    {
        foreach (var item in source)
        {
            if (tryParse(item, out var result))
                yield return result;
        }
    }

    private static bool DefaultTryParse<T>(string value, out T result)
    {
        try
        {
            result = (T)Convert.ChangeType(value, typeof(T));
            return true;
        }
        catch
        {
            result = default!;
            return false;
        }
    }

    public delegate bool TryParseDelegate<T>(string value, out T result);
}