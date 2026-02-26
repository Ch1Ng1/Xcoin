using System.Text.Json;
using System.Linq;
using XCoinMonthChart.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DataFetcher>();
builder.Services.AddSingleton<ChartRenderer>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// serve a tiny transparent PNG for favicon requests to avoid 404 in browser
var _faviconBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=");
app.MapGet("/favicon.ico", () => Results.File(_faviconBytes, "image/png"));

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Json(new { status = "ok", now = DateTime.UtcNow }));

app.MapGet("/chart", async (DataFetcher fetcher, ChartRenderer renderer, ILogger<Program> logger, string? coin) =>
{
    try
    {
        var validCoins = new[] { "bitcoin", "ethereum", "solana", "xrp", "cardano" };
        coin ??= "bitcoin";
        if (!validCoins.Contains(coin.ToLower()))
        {
            coin = "bitcoin";
        }
        logger.LogInformation("Chart request for {Coin}", coin);
        var data = await fetcher.GetMonthlyAveragesAsync(coin);
        var png = renderer.RenderMonthlyAverages(data, coin);
        return Results.File(png, "image/png");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to render chart");
        return Results.Problem(detail: ex.Message);
    }
});

// API: returns monthly average prices per year and month
app.MapGet("/api/monthly-averages", async (DataFetcher fetcher, ILogger<Program> logger, string? coin) =>
{
    try
    {
        var validCoins = new[] { "bitcoin", "ethereum", "solana", "xrp", "cardano" };
        coin ??= "bitcoin";
        if (!validCoins.Contains(coin.ToLower()))
        {
            coin = "bitcoin";
        }
        logger.LogInformation("Monthly averages request for {Coin}", coin);
        var data = await fetcher.GetMonthlyAveragesAsync(coin);
        return Results.Json(data);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to get monthly averages");
        return Results.Problem(detail: ex.Message);
    }
});

app.Run();
