using XCoinMonthChart.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services (each with a single responsibility)
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<CoinValidator>();
builder.Services.AddSingleton<MockPriceProvider>();
builder.Services.AddSingleton<FileCacheService>();
builder.Services.AddSingleton<CoinGeckoClient>();
builder.Services.AddSingleton<DataFetcher>();
builder.Services.AddSingleton<ChartRenderer>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

var coinValidator = app.Services.GetRequiredService<CoinValidator>();

// Serve a tiny transparent PNG for favicon requests to avoid 404 in browser
var faviconBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=");
app.MapGet("/favicon.ico", () => Results.File(faviconBytes, "image/png"));

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.MapGet("/health", () => Results.Json(new { status = "ok", now = DateTime.UtcNow }));

app.MapGet("/chart", async (DataFetcher fetcher, ChartRenderer renderer, ILogger<Program> logger, string? coin) =>
{
    try
    {
        coin = coinValidator.Normalize(coin);
        logger.LogInformation("Chart request for {Coin}", coin);
        var pricesByYear = await fetcher.GetMonthlyAveragesAsync(coin);
        var pngBytes = renderer.RenderMonthlyAverages(pricesByYear, coin);
        return Results.File(pngBytes, "image/png");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to render chart for {Coin}", coin);
        return Results.Problem(detail: "Chart generation failed. Please try again later.");
    }
});

app.MapGet("/api/monthly-averages", async (DataFetcher fetcher, ILogger<Program> logger, string? coin) =>
{
    try
    {
        coin = coinValidator.Normalize(coin);
        logger.LogInformation("Monthly averages request for {Coin}", coin);
        var pricesByYear = await fetcher.GetMonthlyAveragesAsync(coin);
        return Results.Json(pricesByYear);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to get monthly averages for {Coin}", coin);
        return Results.Problem(detail: "Failed to fetch monthly averages. Please try again later.");
    }
});

app.Run();
