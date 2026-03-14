using XCoinBlazor.Models;
using System.Net.Http.Json;

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
    /// Loads monthly averages from JSON file
    /// </summary>
    public async Task<Dictionary<int, Dictionary<int, double>>> LoadMonthlyAveragesAsync(string cryptoName = "bitcoin")
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<Dictionary<int, Dictionary<int, double>>>("data/month-stats.json");
            return data ?? new Dictionary<int, Dictionary<int, double>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return new Dictionary<int, Dictionary<int, double>>();
        }
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