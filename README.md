# Xcoin

A Blazor WebAssembly app showing monthly cryptocurrency price charts for BTC, ETH, SOL, XRP, DOGE, SHIB, and QNT.

## Price data

### Methodology

Historical price data is produced by `scripts/update_price_data.py`. The script downloads **daily USD close prices** from Yahoo Finance (primary) or CryptoCompare (fallback), groups them by calendar month (UTC), and computes the **arithmetic mean** for each month. Only months with at least 25 daily data points are included.

The current year's data is overridden live in the app via the CoinGecko API.

### Data files

All data lives in `XCoinBlazor/wwwroot/data/`:

| File | Coin |
|------|------|
| `month-stats.json` | BTC |
| `eth-month-stats.json` | ETH |
| `solana-month-stats.json` | SOL |
| `xrp-month-stats.json` | XRP |
| `doge-month-stats.json` | DOGE |
| `shib-month-stats.json` | SHIB |
| `qnt-month-stats.json` | QNT |
| `metadata.json` | Generation metadata |

### Re-running the data pipeline

The data is updated automatically on the 1st of every month via the **Update Price Data** GitHub Actions workflow. To run it manually:

1. Go to the **Actions** tab → **Update Price Data** → **Run workflow**.
2. The workflow fetches fresh data, validates it against reference values, and commits the updated JSON files.
3. The commit triggers the GitHub Pages build, which publishes the updated data to `docs/`.

### Important

**Do not edit the JSON price files by hand.** All historical values must be produced by the script from a real data source. Manual edits will be overwritten on the next workflow run.
