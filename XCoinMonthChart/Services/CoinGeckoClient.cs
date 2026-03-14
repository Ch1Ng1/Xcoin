using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace XCoinMonthChart.Services;

/// <summary>
/// Fetches cryptocurrency market data from the CoinGecko public API.
/// Handles retries, timeouts, and response parsing.
/// </summary>
public class CoinGeckoClient
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<CoinGeckoClient> _logger;
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);
    private const int MaxRetries = 3;

    public CoinGeckoClient(IHttpClientFactory httpFactory, ILogger<CoinGeckoClient> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    /// <summary>
    /// Fetches raw market-chart JSON from CoinGecko for the given coin and year range.
    /// Retries up to <see cref="MaxRetries"/> times with exponential back-off.
    /// </summary>
    /// <param name="coinGeckoId">CoinGecko coin identifier (e.g. "ripple" for XRP).</param>
    /// <param name="year">Calendar year to fetch.</param>
    /// <returns>Raw JSON response string, or <c>null</c> if all attempts fail.</returns>
    public async Task<string?> FetchMarketChartJsonAsync(string coinGeckoId, int year)
    {
        var fromTimestamp = new DateTimeOffset(new DateTime(year, 1, 1)).ToUnixTimeSeconds();
        var toTimestamp = new DateTimeOffset(new DateTime(year, 12, 31, 23, 59, 59)).ToUnixTimeSeconds();

        var client = _httpFactory.CreateClient();
        client.Timeout = RequestTimeout;
        if (!client.DefaultRequestHeaders.UserAgent.Any())
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("XCoinMonthChart/1.0 (+https://example)");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        var url = $"https://api.coingecko.com/api/v3/coins/{coinGeckoId}/market_chart/range?vs_currency=usd&from={fromTimestamp}&to={toTimestamp}";

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            await Task.Delay(2000 * attempt);
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API success for {CoinId} {Year} (attempt {Attempt})", coinGeckoId, year, attempt);
                    return json;
                }

                _logger.LogWarning("API returned {StatusCode} for {CoinId} {Year} (attempt {Attempt}/{Max})",
                    (int)response.StatusCode, coinGeckoId, year, attempt, MaxRetries);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("API request timed out for {CoinId} {Year} (attempt {Attempt}/{Max})",
                    coinGeckoId, year, attempt, MaxRetries);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API request failed for {CoinId} {Year} (attempt {Attempt}/{Max})",
                    coinGeckoId, year, attempt, MaxRetries);
            }
        }

        _logger.LogWarning("All {Max} API attempts failed for {CoinId} {Year}", MaxRetries, coinGeckoId, year);
        return null;
    }

    /// <summary>
    /// Parses CoinGecko market-chart JSON and computes monthly average prices for the given year.
    /// </summary>
    /// <param name="json">Raw JSON from the CoinGecko market_chart/range endpoint.</param>
    /// <param name="year">Calendar year used to filter data points.</param>
    /// <returns>Dictionary mapping month (1–12) to average USD price, or <c>null</c> on parse failure.</returns>
    public Dictionary<int, double>? ParseMonthlyAverages(string json, int year)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("prices", out var pricesArray)
                || pricesArray.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning("JSON has no 'prices' array for year {Year}", year);
                return null;
            }

            var pricesByMonth = new Dictionary<int, List<double>>();

            foreach (var entry in pricesArray.EnumerateArray())
            {
                if (entry.ValueKind != JsonValueKind.Array || entry.GetArrayLength() < 2)
                    continue;

                var timestampMs = entry[0].GetInt64();
                var priceUsd = (double)entry[1].GetDecimal();
                var dateUtc = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime;

                if (dateUtc.Year != year)
                    continue;

                if (!pricesByMonth.TryGetValue(dateUtc.Month, out var monthPrices))
                {
                    monthPrices = new List<double>();
                    pricesByMonth[dateUtc.Month] = monthPrices;
                }
                monthPrices.Add(priceUsd);
            }

            var averages = new Dictionary<int, double>();
            foreach (var (month, prices) in pricesByMonth)
            {
                if (prices.Count > 0)
                    averages[month] = prices.Average();
            }

            return averages.Count > 0 ? averages : null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse CoinGecko JSON for year {Year}", year);
            return null;
        }
    }
}
