namespace XCoinMonthChart.Services;

/// <summary>
/// Provides hardcoded historical price data for supported cryptocurrencies.
/// Used for years prior to the current year when live API data is not needed.
/// </summary>
public class MockPriceProvider
{
    /// <summary>
    /// Monthly prices indexed by [coinName][year] → 12-element array (Jan–Dec).
    /// A value of 0 means no data is available for that month.
    /// </summary>
    private static readonly Dictionary<string, Dictionary<int, double[]>> HistoricalPrices = new()
    {
        ["bitcoin"] = new()
        {
            { 2021, new[] { 33000d, 45000, 58000, 63000, 37000, 35000, 33000, 47000, 43000, 61000, 65000, 47000 } },
            { 2022, new[] { 47000d, 44000, 47000, 46000, 38000, 30000, 20000, 24000, 20000, 19000, 17000, 17000 } },
            { 2023, new[] { 17000d, 23000, 28000, 30000, 27000, 30000, 31000, 29000, 26000, 34000, 37000, 42000 } },
            { 2024, new[] { 42000d, 52000, 73000, 64000, 61000, 64000, 58000, 59000, 55000, 63000, 96000, 126000 } },
            { 2025, new[] { 108000d, 95000, 80000, 90000, 95000, 100000, 95000, 85000, 75000, 80000, 85000, 95000 } },
            { 2026, new[] { 95000d, 100000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
        },
        ["ethereum"] = new()
        {
            { 2021, new[] { 2000d, 3000, 3500, 4000, 2500, 2200, 1800, 3200, 2900, 4300, 4700, 3700 } },
            { 2022, new[] { 3700d, 3400, 3500, 3300, 2800, 1800, 1000, 1600, 1300, 1200, 1100, 1200 } },
            { 2023, new[] { 1200d, 1600, 1800, 1900, 1700, 1800, 1900, 1700, 1600, 2000, 2200, 2500 } },
            { 2024, new[] { 2500d, 3200, 3800, 3500, 3300, 3500, 3200, 3300, 3000, 3500, 4000, 4200 } },
            { 2025, new[] { 4200d, 3800, 3500, 4000, 4200, 4500, 4200, 3800, 3300, 3500, 3800, 4200 } },
            { 2026, new[] { 4200d, 4500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
        },
        ["solana"] = new()
        {
            { 2021, new[] { 20d, 50, 100, 150, 200, 180, 140, 160, 130, 200, 250, 170 } },
            { 2022, new[] { 170d, 150, 100, 120, 90, 30, 20, 40, 30, 25, 20, 15 } },
            { 2023, new[] { 15d, 20, 25, 30, 20, 25, 30, 25, 20, 30, 40, 50 } },
            { 2024, new[] { 50d, 80, 120, 150, 140, 160, 130, 140, 120, 150, 180, 200 } },
            { 2025, new[] { 200d, 180, 150, 170, 190, 200, 180, 160, 140, 150, 170, 190 } },
            { 2026, new[] { 190d, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
        },
        ["xrp"] = new()
        {
            { 2021, new[] { 0.5, 0.8, 1.0, 1.2, 0.9, 0.7, 0.6, 1.0, 0.9, 1.3, 1.4, 1.0 } },
            { 2022, new[] { 1.0, 0.9, 0.8, 0.7, 0.6, 0.4, 0.3, 0.5, 0.4, 0.3, 0.3, 0.3 } },
            { 2023, new[] { 0.3, 0.4, 0.5, 0.5, 0.4, 0.5, 0.5, 0.4, 0.4, 0.6, 0.7, 0.8 } },
            { 2024, new[] { 0.8, 0.9, 1.0, 0.9, 0.8, 0.9, 0.8, 0.8, 0.7, 0.9, 1.0, 1.1 } },
            { 2025, new[] { 1.1, 1.0, 0.9, 1.0, 1.1, 1.2, 1.1, 1.0, 0.9, 1.0, 1.1, 1.2 } },
            { 2026, new[] { 1.2, 1.3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
        },
        ["cardano"] = new()
        {
            { 2021, new[] { 1.0, 1.5, 2.0, 2.5, 1.8, 1.5, 1.2, 2.0, 1.8, 2.5, 2.8, 2.0 } },
            { 2022, new[] { 2.0, 1.8, 1.5, 1.3, 1.0, 0.5, 0.3, 0.6, 0.5, 0.4, 0.3, 0.3 } },
            { 2023, new[] { 0.3, 0.4, 0.5, 0.5, 0.4, 0.5, 0.5, 0.4, 0.4, 0.6, 0.7, 0.8 } },
            { 2024, new[] { 0.8, 1.0, 1.2, 1.1, 1.0, 1.1, 1.0, 1.0, 0.9, 1.1, 1.3, 1.4 } },
            { 2025, new[] { 1.4, 1.3, 1.1, 1.2, 1.4, 1.5, 1.4, 1.3, 1.1, 1.2, 1.3, 1.4 } },
            { 2026, new[] { 1.4, 1.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
        }
    };

    /// <summary>
    /// Returns monthly average prices for a given coin and year from hardcoded historical data.
    /// Only months with a price greater than zero are included.
    /// </summary>
    /// <param name="coin">Lowercase coin name (e.g. "bitcoin").</param>
    /// <param name="year">Calendar year.</param>
    /// <returns>Dictionary mapping month number (1–12) to average price, or <c>null</c> if no data exists.</returns>
    public Dictionary<int, double>? GetMonthlyAverages(string coin, int year)
    {
        if (!HistoricalPrices.TryGetValue(coin, out var yearMap) ||
            !yearMap.TryGetValue(year, out var monthlyPrices))
        {
            return null;
        }

        var result = new Dictionary<int, double>();
        for (int month = 1; month <= 12; month++)
        {
            double price = monthlyPrices[month - 1];
            if (price > 0)
                result[month] = price;
        }

        return result.Count > 0 ? result : null;
    }
}
