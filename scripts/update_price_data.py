#!/usr/bin/env python3
"""Download daily crypto prices and compute monthly averages for XCoin Blazor app.

Primary source: Yahoo Finance (yfinance).
Fallback source: CryptoCompare histoday API.
"""

import json
import math
import os
import subprocess
import sys
from datetime import datetime, timezone, timedelta
from pathlib import Path

import requests

try:
    import yfinance as yf
except ImportError:
    yf = None

# ── Configuration ──────────────────────────────────────────────────────────

COINS = {
    "BTC": {
        "yf_ticker": "BTC-USD",
        "cc_symbol": "BTC",
        "json_file": "month-stats.json",
    },
    "ETH": {
        "yf_ticker": "ETH-USD",
        "cc_symbol": "ETH",
        "json_file": "eth-month-stats.json",
    },
    "SOL": {
        "yf_ticker": "SOL-USD",
        "cc_symbol": "SOL",
        "json_file": "solana-month-stats.json",
    },
    "XRP": {
        "yf_ticker": "XRP-USD",
        "cc_symbol": "XRP",
        "json_file": "xrp-month-stats.json",
    },
    "DOGE": {
        "yf_ticker": "DOGE-USD",
        "cc_symbol": "DOGE",
        "json_file": "doge-month-stats.json",
    },
    "SHIB": {
        "yf_ticker": "SHIB-USD",
        "cc_symbol": "SHIB",
        "json_file": "shib-month-stats.json",
    },
    "QNT": {
        "yf_ticker": "QNT-USD",
        "cc_symbol": "QNT",
        "json_file": "qnt-month-stats.json",
    },
}

FIRST_YEAR = 2022
DATA_DIR = Path(__file__).resolve().parent.parent / "XCoinBlazor" / "wwwroot" / "data"
MIN_DAYS_PER_MONTH = 25

# ── Reference values for validation ───────────────────────────────────────

MONTHLY_AVG_CHECKS = [
    ("BTC", 2023, 10, 29749, 0.02),
    ("BTC", 2024, 11, 86569, 0.02),
    ("BTC", 2025, 9, 113171, 0.02),
    ("BTC", 2025, 11, 96282, 0.02),
    ("ETH", 2025, 8, 4252, 0.02),
    ("ETH", 2025, 11, 3204, 0.02),
    ("XRP", 2024, 12, 2.3303, 0.02),
    ("XRP", 2025, 7, 2.8805, 0.02),
    ("DOGE", 2024, 11, 0.32378, 0.02),
    ("DOGE", 2025, 12, 0.13341, 0.02),
]

DAILY_CLOSE_CHECKS = [
    ("SOL", "2023-12-31", 101.44, 0.03),
    ("SOL", "2024-11-30", 237.61, 0.03),
    ("SOL", "2025-11-30", 132.98, 0.03),
    ("BTC", "2025-12-31", 87640, 0.03),
    ("ETH", "2026-02-28", 1964, 0.03),
]

NOV_LT_OCT_2025 = ["BTC", "ETH", "SOL", "XRP", "DOGE"]


# ── Helpers ────────────────────────────────────────────────────────────────

def round_sig(x: float, sig: int = 6) -> float:
    """Round to *sig* significant digits."""
    if x == 0:
        return 0.0
    d = math.ceil(math.log10(abs(x)))
    power = sig - d
    magnitude = 10**power
    return round(x * magnitude) / magnitude


def format_number(x: float) -> str:
    """Format a number without scientific notation, stripping trailing zeros."""
    # Use enough decimal places to preserve 6 significant digits
    if x == 0:
        return "0"
    d = math.ceil(math.log10(abs(x))) if x != 0 else 1
    decimals = max(0, 6 - d)
    formatted = f"{x:.{decimals}f}"
    if "." in formatted:
        formatted = formatted.rstrip("0").rstrip(".")
    return formatted


# ── Data fetching ──────────────────────────────────────────────────────────

def fetch_yfinance(ticker: str, start: str, end: str) -> dict[str, float]:
    """Return {YYYY-MM-DD: close_price} from Yahoo Finance."""
    if yf is None:
        return {}
    try:
        data = yf.download(ticker, start=start, end=end, interval="1d", progress=False, auto_adjust=True)
        if data.empty:
            return {}
        result = {}
        for idx, row in data.iterrows():
            date_str = idx.strftime("%Y-%m-%d") if hasattr(idx, "strftime") else str(idx)[:10]
            close = float(row["Close"].iloc[0]) if hasattr(row["Close"], "iloc") else float(row["Close"])
            if close > 0:
                result[date_str] = close
        return result
    except Exception as e:
        print(f"  yfinance error for {ticker}: {e}")
        return {}


def fetch_cryptocompare(symbol: str, start_ts: int, end_ts: int) -> dict[str, float]:
    """Return {YYYY-MM-DD: close_price} from CryptoCompare histoday."""
    api_key = os.environ.get("CRYPTOCOMPARE_API_KEY", "")
    all_data: dict[str, float] = {}
    # CryptoCompare returns up to 2000 days per call; we need to page
    current_end = end_ts
    while current_end > start_ts:
        url = f"https://min-api.cryptocompare.com/data/v2/histoday?fsym={symbol}&tsym=USD&limit=2000&toTs={current_end}"
        params = {}
        if api_key:
            params["api_key"] = api_key
        try:
            resp = requests.get(url, params=params, timeout=30)
            resp.raise_for_status()
            body = resp.json()
            if body.get("Response") != "Success":
                print(f"  CryptoCompare error for {symbol}: {body.get('Message', 'unknown')}")
                break
            points = body.get("Data", {}).get("Data", [])
            if not points:
                break
            earliest = None
            for p in points:
                ts = p.get("time", 0)
                close = p.get("close", 0)
                if ts < start_ts or close <= 0:
                    continue
                dt = datetime.fromtimestamp(ts, tz=timezone.utc)
                date_str = dt.strftime("%Y-%m-%d")
                all_data[date_str] = close
                if earliest is None or ts < earliest:
                    earliest = ts
            if earliest is None or earliest <= start_ts:
                break
            current_end = earliest - 1
        except Exception as e:
            err_msg = str(e)
            if api_key:
                err_msg = err_msg.replace(api_key, "***")
            print(f"  CryptoCompare error for {symbol}: {err_msg}")
            break
    return all_data


def compute_monthly_averages(daily: dict[str, float]) -> dict[str, dict[int, float]]:
    """Group daily closes by year-month, return {year_str: {month: avg}}."""
    buckets: dict[str, dict[int, list[float]]] = {}
    for date_str, price in daily.items():
        year = date_str[:4]
        month = int(date_str[5:7])
        buckets.setdefault(year, {}).setdefault(month, []).append(price)

    result: dict[str, dict[int, float]] = {}
    for year, months in buckets.items():
        result[year] = {}
        for month, prices in months.items():
            if len(prices) >= MIN_DAYS_PER_MONTH:
                result[year][month] = sum(prices) / len(prices)
    return result


# ── Validation ─────────────────────────────────────────────────────────────

def validate(all_monthly: dict[str, dict[str, dict[int, float]]],
             all_daily: dict[str, dict[str, float]]) -> list[str]:
    """Run all validation checks; return list of error messages."""
    errors: list[str] = []
    now = datetime.now(timezone.utc)
    last_complete_year = now.year - 1

    # Monthly average checks
    for coin, year, month, expected, tol in MONTHLY_AVG_CHECKS:
        monthly = all_monthly.get(coin, {})
        year_str = str(year)
        if year_str not in monthly or month not in monthly[year_str]:
            errors.append(f"Missing {coin} {year}-{month:02d} for monthly avg check")
            continue
        actual = all_monthly[coin].get(str(year), {}).get(month)
        if actual is None:
            errors.append(f"Missing {coin} {year}-{month:02d}")
            continue
        diff = abs(actual - expected) / expected
        status = "PASS" if diff <= tol else "FAIL"
        print(f"  Check {coin} {year}-{month:02d} avg: expected={expected}, actual={round_sig(actual)}, diff={diff:.4%} [{status}]")
        if diff > tol:
            errors.append(f"{coin} {year}-{month:02d} monthly avg {round_sig(actual)} differs from {expected} by {diff:.2%} (tolerance {tol:.0%})")

    # Daily close checks
    for coin, date_str, expected, tol in DAILY_CLOSE_CHECKS:
        daily = all_daily.get(coin, {})
        actual = daily.get(date_str)
        if actual is None:
            # Try nearby dates (weekends/holidays)
            errors.append(f"Missing {coin} daily close for {date_str}")
            continue
        diff = abs(actual - expected) / expected
        status = "PASS" if diff <= tol else "FAIL"
        print(f"  Check {coin} {date_str} close: expected={expected}, actual={round_sig(actual)}, diff={diff:.4%} [{status}]")
        if diff > tol:
            errors.append(f"{coin} {date_str} daily close {round_sig(actual)} differs from {expected} by {diff:.2%} (tolerance {tol:.0%})")

    # Nov < Oct 2025
    for coin in NOV_LT_OCT_2025:
        monthly = all_monthly.get(coin, {}).get("2025", {})
        oct_val = monthly.get(10)
        nov_val = monthly.get(11)
        if oct_val is None or nov_val is None:
            errors.append(f"{coin} missing Oct/Nov 2025 for comparison")
            continue
        if nov_val >= oct_val:
            errors.append(f"{coin} Nov 2025 ({round_sig(nov_val)}) >= Oct 2025 ({round_sig(oct_val)})")
        else:
            print(f"  Check {coin} Nov 2025 < Oct 2025: {round_sig(nov_val)} < {round_sig(oct_val)} [PASS]")

    # No zero/negative, complete years
    for coin in COINS:
        monthly = all_monthly.get(coin, {})
        for year_str, months in monthly.items():
            year_int = int(year_str)
            for month, val in months.items():
                if val <= 0:
                    errors.append(f"{coin} {year_str}-{month:02d} has non-positive price {val}")
            # Check completeness for full years
            if year_int >= FIRST_YEAR and year_int <= last_complete_year:
                if len(months) != 12:
                    errors.append(f"{coin} {year_str} has {len(months)} months, expected 12")
            elif year_int == now.year and now.month == 12:
                # Current year, December — might be complete
                pass

    return errors


# ── Cross-check ────────────────────────────────────────────────────────────

def cross_check(yf_monthly: dict[str, dict[int, float]],
                cc_monthly: dict[str, dict[int, float]],
                coin: str) -> list[str]:
    """Compare monthly averages from two sources, error if >3% diff."""
    errors = []
    for year_str in yf_monthly:
        if year_str not in cc_monthly:
            continue
        for month in yf_monthly[year_str]:
            if month not in cc_monthly[year_str]:
                continue
            yf_val = yf_monthly[year_str][month]
            cc_val = cc_monthly[year_str][month]
            if yf_val == 0:
                continue
            diff = abs(yf_val - cc_val) / yf_val
            if diff > 0.03:
                errors.append(f"{coin} {year_str}-{month:02d}: yfinance={round_sig(yf_val)}, CryptoCompare={round_sig(cc_val)}, diff={diff:.2%}")
    return errors


# ── Main ───────────────────────────────────────────────────────────────────

def main() -> int:
    now = datetime.now(timezone.utc)
    yesterday = now - timedelta(days=1)
    start_date = f"{FIRST_YEAR}-01-01"
    end_date = yesterday.strftime("%Y-%m-%d")
    start_ts = int(datetime(FIRST_YEAR, 1, 1, tzinfo=timezone.utc).timestamp())
    end_ts = int(yesterday.replace(hour=23, minute=59, second=59).timestamp())

    print(f"Fetching daily prices from {start_date} to {end_date}")
    print(f"Data directory: {DATA_DIR}")

    all_monthly: dict[str, dict[str, dict[int, float]]] = {}
    all_daily: dict[str, dict[str, float]] = {}
    source_used = None
    errors: list[str] = []

    for coin, cfg in COINS.items():
        print(f"\n{'='*60}")
        print(f"Processing {coin}...")

        # Primary: Yahoo Finance
        yf_daily = fetch_yfinance(cfg["yf_ticker"], start_date, end_date)
        yf_monthly = compute_monthly_averages(yf_daily) if yf_daily else {}

        # Fallback: CryptoCompare
        cc_daily = fetch_cryptocompare(cfg["cc_symbol"], start_ts, end_ts)
        cc_monthly = compute_monthly_averages(cc_daily) if cc_daily else {}

        # Cross-check if both available
        if yf_monthly and cc_monthly:
            xc_errors = cross_check(yf_monthly, cc_monthly, coin)
            if xc_errors:
                print(f"  Cross-check errors for {coin}:")
                for e in xc_errors:
                    print(f"    {e}")
                errors.extend(xc_errors)

        # Pick source
        if yf_daily:
            daily = yf_daily
            monthly = yf_monthly
            if source_used is None:
                source_used = "Yahoo Finance (yfinance)"
            print(f"  Using Yahoo Finance: {len(daily)} daily points")
        elif cc_daily:
            daily = cc_daily
            monthly = cc_monthly
            if source_used is None:
                source_used = "CryptoCompare"
            print(f"  Using CryptoCompare: {len(daily)} daily points")
        else:
            errors.append(f"No data available for {coin}")
            continue

        all_daily[coin] = daily
        all_monthly[coin] = monthly

    if not all_monthly:
        print("\nERROR: No data fetched for any coin. Cannot proceed.")
        print("If network is restricted, commit script/workflow/app changes without modifying JSON files.")
        return 1

    # Cross-check errors are fatal
    if errors:
        print("\nCross-check errors detected:")
        for e in errors:
            print(f"  {e}")
        return 1

    # Validate
    print(f"\n{'='*60}")
    print("Running validation checks...")
    val_errors = validate(all_monthly, all_daily)
    if val_errors:
        print("\nValidation FAILED:")
        for e in val_errors:
            print(f"  ✗ {e}")
        return 1
    print("All validation checks passed!")

    # Write JSON files
    print(f"\n{'='*60}")
    print("Writing JSON files...")
    coverage: dict[str, dict[str, str]] = {}

    for coin, cfg in COINS.items():
        monthly = all_monthly.get(coin, {})
        if not monthly:
            continue

        # Build ordered output
        output: dict[str, list] = {}
        sorted_years = sorted(monthly.keys(), key=int)
        first_month_str = None
        last_month_str = None

        for year_str in sorted_years:
            months = monthly[year_str]
            # Only write consecutive months starting from 1
            values = []
            for m in range(1, 13):
                if m in months:
                    values.append(round_sig(months[m]))
                else:
                    break
            if values:
                output[year_str] = values
                if first_month_str is None:
                    first_month_str = f"{year_str}-01"
                last_month_str = f"{year_str}-{len(values):02d}"

        coverage[coin] = {"first": first_month_str or "", "last": last_month_str or ""}

        # Write with custom formatting to avoid scientific notation
        filepath = DATA_DIR / cfg["json_file"]
        with open(filepath, "w", encoding="utf-8", newline="\n") as f:
            f.write("{\n")
            entries = list(output.items())
            for i, (year_str, values) in enumerate(entries):
                formatted_values = ", ".join(format_number(v) for v in values)
                comma = "," if i < len(entries) - 1 else ""
                f.write(f'  "{year_str}": [{formatted_values}]{comma}\n')
            f.write("}\n")

        print(f"  Wrote {filepath.name}: {len(output)} years")

        # Print summary table
        print(f"\n  {coin} summary:")
        for year_str, values in output.items():
            formatted = [format_number(v) for v in values]
            print(f"    {year_str}: {', '.join(formatted)}")

    # Write metadata
    try:
        git_sha = subprocess.check_output(
            ["git", "rev-parse", "HEAD"], cwd=str(DATA_DIR), text=True
        ).strip()
    except Exception:
        git_sha = "unknown"

    metadata = {
        "source": source_used or "unknown",
        "method": "monthly average of daily close prices, USD, UTC calendar months",
        "generated_at": now.strftime("%Y-%m-%dT%H:%M:%SZ"),
        "coverage": coverage,
        "script": "scripts/update_price_data.py",
        "commit": git_sha,
    }

    meta_path = DATA_DIR / "metadata.json"
    with open(meta_path, "w", encoding="utf-8", newline="\n") as f:
        json.dump(metadata, f, indent=2, ensure_ascii=False)
        f.write("\n")
    print(f"\n  Wrote {meta_path.name}")

    print(f"\nDone. Source: {source_used}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
