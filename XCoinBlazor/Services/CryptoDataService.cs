using XCoinBlazor.Models;

namespace XCoinBlazor.Services;

/// <summary>
/// Service for loading and processing cryptocurrency data from hardcoded data
/// </summary>
public class CryptoDataService
{
    /// <summary>
    /// Loads monthly averages from hardcoded data
    /// </summary>
    public async Task<Dictionary<int, Dictionary<int, double>>> LoadMonthlyAveragesAsync(string cryptoName = "bitcoin")
    {
        // Simulate async operation
        await Task.Delay(100);

        // Base data for Bitcoin
        var baseData = new Dictionary<int, Dictionary<int, double>>
        {
            {
                2019, new Dictionary<int, double>
                {
                    {1, 115.78}, {2, 142.63}, {3, 146.26}, {4, 171.48}, {5, 168.87}, {6, 186.97},
                    {7, 186.16}, {8, 178.41}, {9, 174.96}, {10, 158.8}, {11, 148.23}, {12, 142.09}
                }
            },
            {
                2020, new Dictionary<int, double>
                {
                    {1, 317.22}, {2, 345.69}, {3, 359.51}, {4, 368.54}, {5, 379.88}, {6, 395.23},
                    {7, 390.45}, {8, 383.66}, {9, 377.88}, {10, 371.5}, {11, 365.25}, {12, 348.93}
                }
            },
            {
                2021, new Dictionary<int, double>
                {
                    {1, 516.13}, {2, 539.54}, {3, 554.86}, {4, 569.48}, {5, 590.58}, {6, 590.03},
                    {7, 604.98}, {8, 598.69}, {9, 598.5}, {10, 575.2}, {11, 552.78}, {12, 552.1}
                }
            },
            {
                2022, new Dictionary<int, double>
                {
                    {1, 731.86}, {2, 738.9}, {3, 771.21}, {4, 788.95}, {5, 803.58}, {6, 815.05},
                    {7, 800.39}, {8, 798.1}, {9, 799.96}, {10, 787.23}, {11, 764.94}, {12, 746.2}
                }
            },
            {
                2023, new Dictionary<int, double>
                {
                    {1, 920.17}, {2, 943.7}, {3, 984.21}, {4, 1002.64}, {5, 999.15}, {6, 1019.58},
                    {7, 1023.81}, {8, 1024.22}, {9, 1012.51}, {10, 991.3}, {11, 969.06}, {12, 954.37}
                }
            },
            {
                2024, new Dictionary<int, double>
                {
                    {1, 925.82}, {2, 893.84}, {3, 880.57}, {4, 855.81}, {5, 834.5}, {6, 812.28},
                    {7, 782.82}, {8, 778.38}, {9, 770.26}, {10, 792.63}, {11, 799.79}, {12, 801.01}
                }
            },
            {
                2025, new Dictionary<int, double>
                {
                    {1, 822.93}, {2, 842.59}, {3, 874.88}, {4, 920.17}, {5, 943.7}, {6, 984.21},
                    {7, 1002.64}, {8, 999.15}, {9, 1019.58}, {10, 1023.81}, {11, 1024.22}, {12, 1012.51}
                }
            },
            {
                2026, new Dictionary<int, double>
                {
                    {1, 991.3}, {2, 969.06}, {3, 954.37} // Up to March 2026
                }
            }
        };

        // Apply multiplier based on cryptocurrency
        double multiplier = cryptoName.ToLower() switch
        {
            "bitcoin" => 1.0,
            "ethereum" => 0.1,
            "solana" => 0.01,
            "xrp" => 0.001,
            "cardano" => 0.005,
            _ => 1.0
        };

        var adjustedData = new Dictionary<int, Dictionary<int, double>>();
        foreach (var yearEntry in baseData)
        {
            var year = yearEntry.Key;
            var monthlyPrices = new Dictionary<int, double>();
            foreach (var monthEntry in yearEntry.Value)
            {
                monthlyPrices[monthEntry.Key] = Math.Round(monthEntry.Value * multiplier, 2);
            }
            adjustedData[year] = monthlyPrices;
        }

        return adjustedData;
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