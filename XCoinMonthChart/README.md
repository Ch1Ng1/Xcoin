# XCoin — Cryptocurrency Monthly Price Tracker

A lightweight ASP.NET Core web application that visualizes monthly average cryptocurrency prices across multiple years. Compare **Bitcoin**, **Ethereum**, **Solana**, **XRP**, and **Cardano** side-by-side with interactive Chart.js graphs and server-rendered ScottPlot fallback images.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

---

## Features

- **Multi-coin support** — switch between Bitcoin, Ethereum, Solana, XRP, and Cardano
- **5-year comparison** — line chart overlaying each year's monthly averages
- **Live data** — current-year prices fetched from the [CoinGecko API](https://www.coingecko.com/en/api) with automatic retry and 24-hour disk cache
- **Client-side rendering** — interactive Chart.js chart with tooltips, zoom-ready labels, and dual Y-axes
- **Server-side fallback** — ScottPlot-generated PNG served at `/chart` when JavaScript is unavailable
- **Clean architecture** — SOLID-compliant service layer with separated concerns (validation, caching, API client, mock data, rendering)
- **Error resilience** — try/catch at every boundary, structured logging, graceful fallbacks for empty data / API failures / timeouts

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Minimal API |
| Chart (client) | [Chart.js 4](https://www.chartjs.org/) |
| Chart (server) | [ScottPlot 4](https://scottplot.net/) |
| Data source | CoinGecko REST API (free tier) |
| Caching | File-based JSON cache (24h TTL) |
| Frontend | Bootstrap 5, vanilla JS |

## Project Structure

```
Xcoin/
├── XCoinMonthChart/
│   ├── Program.cs                  # App startup, routes, DI registration
│   ├── Services/
│   │   ├── CoinValidator.cs        # Validates & normalizes coin names
│   │   ├── CoinGeckoClient.cs      # HTTP calls to CoinGecko with retry logic
│   │   ├── FileCacheService.cs     # Disk-based JSON cache with TTL
│   │   ├── MockPriceProvider.cs    # Historical price data (2021–2025)
│   │   ├── DataFetcher.cs          # Orchestrates mock + live data fetching
│   │   └── ChartRenderer.cs        # ScottPlot PNG chart rendering
│   └── wwwroot/
│       ├── index.html              # Bootstrap UI with coin selector
│       └── site.js                 # Chart.js rendering + fetch with timeout
├── start-server.ps1                # PowerShell launch script
├── start-server.bat                # Windows batch launch script
└── Xcoin.sln
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

### Run

```bash
git clone https://github.com/Ch1Ng1/Xcoin.git
cd Xcoin/XCoinMonthChart
dotnet run
```

The app starts on `http://localhost:5000` by default. Open it in your browser.

Alternatively, use the included launch scripts:

```powershell
# PowerShell
.\start-server.ps1

# cmd
start-server.bat
```

### API Endpoints

| Endpoint | Description |
|---|---|
| `GET /` | Web UI with interactive chart |
| `GET /api/monthly-averages?coin=bitcoin` | JSON: monthly averages by year |
| `GET /chart?coin=bitcoin` | Server-rendered PNG chart |
| `GET /health` | Health check (`{ status: "ok" }`) |

**Supported coins:** `bitcoin`, `ethereum`, `solana`, `xrp`, `cardano`

## Architecture

The service layer follows **SOLID principles**:

- **`CoinValidator`** — Single source of truth for supported coins, name normalization, and CoinGecko ID mapping
- **`FileCacheService`** — Read/write JSON to disk with configurable TTL (Open/Closed — swap for Redis without touching callers)
- **`MockPriceProvider`** — Supplies historical data for past years (can be replaced with a database)
- **`CoinGeckoClient`** — Handles HTTP requests, retries, timeouts, and JSON parsing for the CoinGecko API
- **`DataFetcher`** — Orchestrator that delegates to the above services; contains no I/O or business logic of its own
- **`ChartRenderer`** — Converts price dictionaries to PNG bytes via ScottPlot

## Error Handling

- All API and disk I/O wrapped in `try/catch` with structured `ILogger` output
- HTTP client has a 15-second timeout per request with 3 retries and exponential back-off
- Frontend `fetch` uses `AbortController` with a 15-second timeout
- Fallback chain: JSON API → server-rendered PNG → error message in UI
- Empty/malformed JSON responses are caught and logged, never crash the app

## Roadmap

- [ ] Unit & integration tests
- [ ] Docker container
- [ ] Add more coins dynamically from CoinGecko's coin list
- [ ] Date range picker in the UI
- [ ] Redis or SQLite cache option
- [ ] GitHub Pages demo with static export

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss the idea.

## License

[MIT](https://opensource.org/licenses/MIT) — free to use and modify.