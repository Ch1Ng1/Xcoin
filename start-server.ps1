param(
    [string]$Url = "http://localhost:5000"
)

$proj = Join-Path $PSScriptRoot "XCoinMonthChart\XCoinMonthChart.csproj"
if (-not (Test-Path $proj)) {
    Write-Error "Project file not found: $proj"
    exit 1
}

Write-Host "Starting XCoinMonthChart using project:`n  $proj`nListening on $Url"
dotnet run --project $proj --urls $Url
