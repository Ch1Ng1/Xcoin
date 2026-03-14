using XCoinBlazor.Models;

namespace XCoinBlazor.Services;

/// <summary>
/// Service for loading and processing cryptocurrency data from hardcoded data
/// </summary>
public class CryptoDataService
{

    /// <summary>
    /// Loads monthly statistics from hardcoded data
    /// </summary>
    public async Task<MonthlyStatsData> LoadMonthlyStatsAsync()
    {
        // Simulate async operation
        await Task.Delay(100);

        var data = new Dictionary<int, MonthlyStats>
        {
            {
                2019, new MonthlyStats
                {
                    TotalDays = 28,
                    Present = 28,
                    Missing = 0,
                    Min = 14.51,
                    Max = 186.97,
                    Avg = 100.8568,
                    Median = 97.48500000000001
                }
            },
            {
                2020, new MonthlyStats
                {
                    TotalDays = 29,
                    Present = 29,
                    Missing = 0,
                    Min = 206.5,
                    Max = 395.23,
                    Avg = 301.3721,
                    Median = 297.04
                }
            },
            {
                2021, new MonthlyStats
                {
                    TotalDays = 28,
                    Present = 28,
                    Missing = 0,
                    Min = 401.42,
                    Max = 604.98,
                    Avg = 497.2868,
                    Median = 495.95
                }
            },
            {
                2022, new MonthlyStats
                {
                    TotalDays = 28,
                    Present = 28,
                    Missing = 0,
                    Min = 590.81,
                    Max = 815.05,
                    Avg = 699.7129,
                    Median = 699.4200000000001
                }
            },
            {
                2023, new MonthlyStats
                {
                    TotalDays = 28,
                    Present = 28,
                    Missing = 0,
                    Min = 770.26,
                    Max = 1024.22,
                    Avg = 900.3818,
                    Median = 895.85
                }
            }
        };

        return new MonthlyStatsData { Years = data };
    }

    /// <summary>
    /// Loads daily prices from hardcoded data
    /// </summary>
    public async Task<DailyPricesData> LoadDailyPricesAsync()
    {
        // Simulate async operation
        await Task.Delay(100);

        var data = new Dictionary<int, List<double>>
        {
            {
                2019, new List<double>
                {
                    115.78, 142.63, 146.26, 171.48, 168.87, 186.97, 186.16, 178.41,
                    174.96, 158.8, 148.23, 142.09, 126.75, 99.29, 87.15, 69.07,
                    51.83, 28.94, 19.56, 14.51, 25.58, 23.88, 28.8, 29.96,
                    52.9, 59.57, 89.88, 95.68
                }
            },
            {
                2020, new List<double>
                {
                    317.22, 345.69, 359.51, 368.54, 379.88, 395.23, 390.45, 383.66,
                    377.88, 371.5, 365.25, 348.93, 335.58, 310.74, 283.23, 279.62,
                    253.93, 238.2, 218.91, 222.17, 215.2, 206.5, 218.79, 228.15,
                    230.18, 253.27, 257.78, 286.76, 297.04
                }
            },
            {
                2021, new List<double>
                {
                    516.13, 539.54, 554.86, 569.48, 590.58, 590.03, 604.98, 598.69,
                    598.5, 575.2, 552.78, 552.1, 522.36, 499.51, 472.45, 454.74,
                    436.76, 417.49, 404.47, 401.42, 402.53, 404.69, 402.59, 412.69,
                    429.55, 458.27, 469.25, 492.39
                }
            },
            {
                2022, new List<double>
                {
                    731.86, 738.9, 771.21, 788.95, 803.58, 815.05, 800.39, 798.1,
                    799.96, 787.23, 764.94, 746.2, 722.45, 697.15, 677.45, 645.35,
                    624.39, 621.07, 594.8, 599.61, 590.81, 592.29, 610.55, 616.07,
                    622.41, 649.47, 680.03, 701.69
                }
            },
            {
                2023, new List<double>
                {
                    920.17, 943.7, 984.21, 1002.64, 999.15, 1019.58, 1023.81, 1024.22,
                    1012.51, 991.3, 969.06, 954.37, 925.82, 893.84, 880.57, 855.81,
                    834.5, 812.28, 782.82, 778.38, 770.26, 792.63, 799.79, 801.01,
                    822.93, 842.59, 874.88
                }
            }
        };

        return new DailyPricesData { Years = data };
    }

    /// <summary>
    /// Processes daily prices into monthly averages
    /// </summary>
    public Dictionary<int, Dictionary<int, double>> ProcessMonthlyAverages(DailyPricesData dailyData)
    {
        var result = new Dictionary<int, Dictionary<int, double>>();

        foreach (var yearEntry in dailyData.Years)
        {
            var year = yearEntry.Key;
            var dailyPrices = yearEntry.Value;

            // For now, assume the daily prices represent monthly prices
            // We'll distribute them across 12 months or use them as-is
            var monthlyPrices = new Dictionary<int, double>();

            // If we have exactly 12 prices, use them as monthly prices
            if (dailyPrices.Count == 12)
            {
                for (int month = 1; month <= 12; month++)
                {
                    monthlyPrices[month] = dailyPrices[month - 1];
                }
            }
            else
            {
                // If we have a different number of prices, distribute them evenly across months
                // or use the first 12 prices as monthly prices
                int monthsToFill = Math.Min(12, dailyPrices.Count);
                for (int month = 1; month <= monthsToFill; month++)
                {
                    monthlyPrices[month] = dailyPrices[month - 1];
                }
            }

            if (monthlyPrices.Any())
            {
                result[year] = monthlyPrices;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets available cryptocurrencies
    /// </summary>
    public List<CryptoCurrency> GetAvailableCryptocurrencies()
    {
        return new List<CryptoCurrency>
        {
            new() { Name = "bitcoin", DisplayName = "Bitcoin", Symbol = "BTC" },
            new() { Name = "ethereum", DisplayName = "Ethereum", Symbol = "ETH" },
            new() { Name = "solana", DisplayName = "Solana", Symbol = "SOL" },
            new() { Name = "xrp", DisplayName = "XRP", Symbol = "XRP" },
            new() { Name = "cardano", DisplayName = "Cardano", Symbol = "ADA" }
        };
    }

    /// <summary>
    /// Calculates statistics for the given monthly data
    /// </summary>
    public (double min, double max, double avg, int totalMonths) CalculateStats(Dictionary<int, Dictionary<int, double>> data)
    {
        var allPrices = data.Values.SelectMany(year => year.Values).Where(p => p > 0).ToList();

        if (!allPrices.Any())
            return (0, 0, 0, 0);

        return (allPrices.Min(), allPrices.Max(), allPrices.Average(), allPrices.Count);
    }

    private bool IsLeapYear(int year)
    {
        return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
    }
}