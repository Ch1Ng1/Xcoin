using System.Drawing;
using System.Drawing.Imaging;
using ScottPlot;
using Microsoft.Extensions.Logging;

namespace XCoinMonthChart.Services;

/// <summary>
/// Renders cryptocurrency monthly-average price data as PNG chart images
/// using ScottPlot for multi-year line comparison.
/// </summary>
public class ChartRenderer
{
    private readonly ILogger<ChartRenderer> _logger;

    private static readonly Color[] YearColors =
    {
        Color.Red, Color.Blue, Color.Yellow,
        Color.Green, Color.Purple, Color.Orange
    };

    public ChartRenderer(ILogger<ChartRenderer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Renders a PNG chart showing monthly average prices per year.
    /// Returns a placeholder image when <paramref name="pricesByYear"/> is empty or null.
    /// </summary>
    /// <param name="pricesByYear">Outer key = year, inner key = month (1–12), value = avg USD price.</param>
    /// <param name="coin">Coin name used in the chart title (e.g. "bitcoin").</param>
    /// <returns>PNG image bytes.</returns>
    public byte[] RenderMonthlyAverages(Dictionary<int, Dictionary<int, double>> pricesByYear, string coin = "bitcoin")
    {
        if (string.IsNullOrWhiteSpace(coin))
        {
            _logger.LogWarning("Coin parameter was null/empty, defaulting to bitcoin");
            coin = "bitcoin";
        }

        if (pricesByYear == null || pricesByYear.Count == 0)
        {
            _logger.LogWarning("No data provided for chart rendering (coin={Coin})", coin);
            return RenderPlaceholderPng("No data available.");
        }

        var allPoints = CollectDataPoints(pricesByYear);

        if (allPoints.Count == 0)
        {
            _logger.LogWarning("Data dictionary was not empty but produced zero points (coin={Coin})", coin);
            return RenderPlaceholderPng("No data points available.");
        }

        _logger.LogInformation("Rendering chart for {Coin}: {YearCount} years, {PointCount} points",
            coin, pricesByYear.Count, allPoints.Count);

        return RenderLineChart(pricesByYear, coin);
    }

    /// <summary>
    /// Flattens year→month→price data into a sorted list of (date, price) tuples.
    /// </summary>
    private static List<(DateTime date, double price)> CollectDataPoints(
        Dictionary<int, Dictionary<int, double>> pricesByYear)
    {
        var points = new List<(DateTime date, double price)>();
        foreach (var (year, months) in pricesByYear)
        {
            foreach (var (month, averagePrice) in months)
            {
                points.Add((new DateTime(year, month, 1), averagePrice));
            }
        }
        points.Sort((a, b) => a.date.CompareTo(b.date));
        return points;
    }

    /// <summary>
    /// Produces the actual ScottPlot line chart PNG.
    /// </summary>
    private byte[] RenderLineChart(Dictionary<int, Dictionary<int, double>> pricesByYear, string coin)
    {
        var plot = new ScottPlot.Plot(800, 400);
        plot.Style(figureBackground: Color.White, dataBackground: Color.White);

        int colorIndex = 0;
        foreach (var (year, monthlyPrices) in pricesByYear.OrderBy(kv => kv.Key))
        {
            var sortedMonths = monthlyPrices.OrderBy(kv => kv.Key).ToList();
            if (sortedMonths.Count == 0)
                continue;

            double[] xDates = sortedMonths.Select(m => new DateTime(year, m.Key, 1).ToOADate()).ToArray();
            double[] yPrices = sortedMonths.Select(m => m.Value).ToArray();
            var lineColor = YearColors[colorIndex % YearColors.Length];

            plot.AddScatter(xDates, yPrices, color: lineColor, markerSize: 0, lineWidth: 2, label: year.ToString());
            colorIndex++;
        }

        plot.Grid(enable: true, lineStyle: ScottPlot.LineStyle.Solid, color: Color.LightGray);
        plot.XLabel("Date");
        plot.YLabel("Average Price (USD)");
        plot.Layout(right: 0.82f, left: 60, top: 60);
        plot.Title($"{CoinValidator.ToDisplayName(coin)} Monthly Average Prices by Year");
        plot.XAxis.DateTimeFormat(true);
        plot.Legend();

        using var bitmap = plot.Render();
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    /// <summary>
    /// Renders a simple white 800×200 PNG with a centred text message.
    /// Used as a fallback when there is no data to chart.
    /// </summary>
    private static byte[] RenderPlaceholderPng(string message)
    {
        using var bitmap = new Bitmap(800, 200);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        using var font = new Font("Segoe UI", 14);
        using var brush = new SolidBrush(Color.Black);
        graphics.DrawString(message, font, brush, new PointF(12, 80));
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
