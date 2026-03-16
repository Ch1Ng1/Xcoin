namespace XCoinBlazor.Models;

/// <summary>
/// Monthly price data from JSON
/// </summary>
public class MonthStat
{
    public string Month { get; set; } = string.Empty;
    public decimal Price { get; set; }
}