# XCoinMonthChart

Minimal .NET 8 app that renders a lightweight PNG line chart comparing cryptocurrency monthly average prices for the last 5 years, with each year displayed in a different color.

Prerequisites:
- .NET 8 SDK

Run locally (Windows):

```powershell
cd c:\xampp\htdocs\Xcoin\XCoinMonthChart
dotnet run

# Open http://localhost:5000 in a browser
```

Notes:
- Data is fetched from CoinGecko and cached to the `cache/` folder.
- Chart shows monthly averages for each of the last 5 years as separate lines.
- Supports multiple cryptocurrencies: Bitcoin, Ethereum, Solana, XRP, Cardano.
