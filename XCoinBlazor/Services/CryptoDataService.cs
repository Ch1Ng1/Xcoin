using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using XCoinBlazor.Models;

namespace XCoinBlazor.Services;

/// <summary>
/// Service for loading and processing cryptocurrency data from JSON files
/// </summary>
public class CryptoDataService
{
    private readonly HttpClient _httpClient;

    public CryptoDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Loads monthly statistics from embedded month-stats.json
    /// </summary>
    public async Task<MonthlyStatsData> LoadMonthlyStatsAsync()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allResources = assembly.GetManifestResourceNames();
            Console.WriteLine($"Available resources: {string.Join(", ", allResources)}");

            var resourceName = "XCoinBlazor.wwwroot.data.month-stats.json";
            Console.WriteLine($"Looking for resource: {resourceName}");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("Embedded resource not found, trying HTTP fallback");
                return await LoadMonthlyStatsFromHttpAsync();
            }

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            Console.WriteLine($"Loaded JSON length: {json.Length}");
            var data = JsonSerializer.Deserialize<Dictionary<int, MonthlyStats>>(json);
            return new MonthlyStatsData { Years = data ?? new Dictionary<int, MonthlyStats>() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading monthly stats: {ex.Message}");
            return new MonthlyStatsData();
        }
    }

    private async Task<MonthlyStatsData> LoadMonthlyStatsFromHttpAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<Dictionary<int, MonthlyStats>>("data/month-stats.json");
            return new MonthlyStatsData { Years = data ?? new Dictionary<int, MonthlyStats>() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTTP fallback failed: {ex.Message}");
            return new MonthlyStatsData();
        }
    }

    /// <summary>
    /// Loads daily prices from embedded resp.json
    /// </summary>
    public async Task<DailyPricesData> LoadDailyPricesAsync()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allResources = assembly.GetManifestResourceNames();
            Console.WriteLine($"Available resources: {string.Join(", ", allResources)}");

            var resourceName = "XCoinBlazor.wwwroot.data.resp.json";
            Console.WriteLine($"Looking for resource: {resourceName}");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine("Embedded resource not found, trying HTTP fallback");
                return await LoadDailyPricesFromHttpAsync();
            }

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            Console.WriteLine($"Loaded JSON length: {json.Length}");
            var data = JsonSerializer.Deserialize<Dictionary<int, List<double>>>(json);
            return new DailyPricesData { Years = data ?? new Dictionary<int, List<double>>() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading daily prices: {ex.Message}");
            return new DailyPricesData();
        }
    }

    private async Task<DailyPricesData> LoadDailyPricesFromHttpAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<Dictionary<int, List<double>>>("data/resp.json");
            return new DailyPricesData { Years = data ?? new Dictionary<int, List<double>>() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTTP fallback failed: {ex.Message}");
            return new DailyPricesData();
        }
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

            var monthlyPrices = new Dictionary<int, double>();
            var daysInMonth = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // Adjust for leap year
            if (IsLeapYear(year)) daysInMonth[1] = 29;

            int dayIndex = 0;
            for (int month = 1; month <= 12; month++)
            {
                int daysInCurrentMonth = daysInMonth[month - 1];
                if (dayIndex + daysInCurrentMonth > dailyPrices.Count)
                    break;

                var monthPrices = dailyPrices.Skip(dayIndex).Take(daysInCurrentMonth).Where(p => p > 0).ToList();
                if (monthPrices.Any())
                {
                    monthlyPrices[month] = monthPrices.Average();
                }

                dayIndex += daysInCurrentMonth;
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