namespace XCoinMonthChart.Services;

/// <summary>
/// Validates and normalizes cryptocurrency coin identifiers.
/// </summary>
public class CoinValidator
{
    private static readonly HashSet<string> SupportedCoins = new(StringComparer.OrdinalIgnoreCase)
    {
        "bitcoin", "ethereum", "solana", "xrp", "cardano"
    };

    private const string DefaultCoin = "bitcoin";

    /// <summary>
    /// Maps a user-facing coin name to its CoinGecko API identifier.
    /// </summary>
    private static readonly Dictionary<string, string> CoinGeckoIdMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bitcoin"] = "bitcoin",
        ["ethereum"] = "ethereum",
        ["solana"] = "solana",
        ["xrp"] = "ripple",
        ["cardano"] = "cardano"
    };

    /// <summary>
    /// Returns <c>true</c> if <paramref name="coin"/> is a supported cryptocurrency name.
    /// </summary>
    public bool IsValid(string? coin)
        => !string.IsNullOrWhiteSpace(coin) && SupportedCoins.Contains(coin.Trim());

    /// <summary>
    /// Normalizes the coin name to lowercase. Returns "bitcoin" for invalid or missing values.
    /// </summary>
    public string Normalize(string? coin)
    {
        if (string.IsNullOrWhiteSpace(coin))
            return DefaultCoin;

        var trimmed = coin.Trim().ToLowerInvariant();
        return SupportedCoins.Contains(trimmed) ? trimmed : DefaultCoin;
    }

    /// <summary>
    /// Converts a user-facing coin name to its CoinGecko API identifier (e.g. "xrp" → "ripple").
    /// </summary>
    public string ToCoinGeckoId(string coin)
        => CoinGeckoIdMap.TryGetValue(coin, out var id) ? id : coin;

    /// <summary>
    /// Formats a coin name for display with the first letter capitalized (e.g. "bitcoin" → "Bitcoin").
    /// </summary>
    public static string ToDisplayName(string coin)
    {
        if (string.IsNullOrEmpty(coin)) return "Bitcoin";
        return char.ToUpper(coin[0]) + coin[1..];
    }
}
