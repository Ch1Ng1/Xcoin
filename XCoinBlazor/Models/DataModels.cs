namespace XCoinBlazor.Models;

/// <summary>
/// Represents monthly statistics for a cryptocurrency
/// </summary>
public class MonthlyStats
{
    public int TotalDays { get; set; }
    public int Present { get; set; }
    public int Missing { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Avg { get; set; }
    public double Median { get; set; }
}

/// <summary>
/// Contains all monthly statistics by year
/// </summary>
public class MonthlyStatsData
{
    public Dictionary<int, MonthlyStats> Years { get; set; } = new();
}

///// <summary>
///// Contains daily prices for a year
///// </summary>
//public class DailyPricesData
//{
//    public Dictionary<int, List<double>> Years { get; set; } = new();
//}

/// <summary>
/// Processed monthly data for charts
/// </summary>
public class MonthlyData
{
    public int Year { get; set; }
    public Dictionary<int, double> MonthlyPrices { get; set; } = new();
}

/// <summary>
/// Cryptocurrency information
/// </summary>
public class CryptoCurrency
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}

/// <summary>
/// Represents a monthly Bitcoin price statistic
/// </summary>
public class MonthStat
{
    public string? Month { get; set; }
    public decimal Price { get; set; }
}