using Microsoft.Extensions.Logging;

namespace XCoinMonthChart.Services;

/// <summary>
/// Orchestrates fetching monthly average cryptocurrency prices across multiple years.
/// Delegates to <see cref="MockPriceProvider"/> for historical data and
/// <see cref="CoinGeckoClient"/> (with <see cref="FileCacheService"/>) for the current year.
/// </summary>
public class DataFetcher
{
    private readonly CoinValidator _coinValidator;
    private readonly MockPriceProvider _mockProvider;
    private readonly CoinGeckoClient _apiClient;
    private readonly FileCacheService _cache;
    private readonly ILogger<DataFetcher> _logger;

    private const int YearsOfHistory = 4;

    public DataFetcher(
        CoinValidator coinValidator,
        MockPriceProvider mockProvider,
        CoinGeckoClient apiClient,
        FileCacheService cache,
        ILogger<DataFetcher> logger)
    {
        _coinValidator = coinValidator;
        _mockProvider = mockProvider;
        _apiClient = apiClient;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Returns monthly average prices for the last 5 years (current year included).
    /// Historical years use mock data; the current year queries CoinGecko with disk caching.
    /// </summary>
    /// <param name="coin">Coin name (e.g. "bitcoin"). Normalized automatically.</param>
    /// <returns>
    /// Outer key = year, inner key = month (1–12), value = average USD price.
    /// Returns an empty dictionary if no data is available at all.
    /// </returns>
    public async Task<Dictionary<int, Dictionary<int, double>>> GetMonthlyAveragesAsync(string coin = "bitcoin")
    {
        coin = _coinValidator.Normalize(coin);
        var pricesByYear = new Dictionary<int, Dictionary<int, double>>();
        int currentYear = DateTime.UtcNow.Year;

        for (int year = currentYear - YearsOfHistory; year <= currentYear; year++)
        {
            try
            {
                var monthlyAverages = (year == currentYear)
                    ? await FetchLiveYearAsync(coin, year)
                    : _mockProvider.GetMonthlyAverages(coin, year);

                if (monthlyAverages is { Count: > 0 })
                {
                    pricesByYear[year] = monthlyAverages;
                    _logger.LogDebug("Loaded {Months} months for {Coin} {Year}", monthlyAverages.Count, coin, year);
                }
                else
                {
                    _logger.LogWarning("No data for {Coin} {Year}", coin, year);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data for {Coin} {Year}, skipping", coin, year);
            }
        }

        if (pricesByYear.Count == 0)
            _logger.LogWarning("No data found for {Coin} across all years", coin);

        return pricesByYear;
    }

    /// <summary>
    /// Fetches live data for the current year: tries cache first, then CoinGecko API.
    /// </summary>
    private async Task<Dictionary<int, double>?> FetchLiveYearAsync(string coin, int year)
    {
        var coinGeckoId = _coinValidator.ToCoinGeckoId(coin);
        var cacheKey = $"{coinGeckoId}-{year}";

        var cachedJson = await _cache.TryReadAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedResult = _apiClient.ParseMonthlyAverages(cachedJson, year);
            if (cachedResult is { Count: > 0 })
                return cachedResult;
        }

        var freshJson = await _apiClient.FetchMarketChartJsonAsync(coinGeckoId, year);
        if (string.IsNullOrWhiteSpace(freshJson))
            return null;

        await _cache.WriteAsync(cacheKey, freshJson);
        return _apiClient.ParseMonthlyAverages(freshJson, year);
    }
}
