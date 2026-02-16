@echo off
setlocal
set PROJ=%~dp0XCoinMonthChart\XCoinMonthChart.csproj
if not exist "%PROJ%" (
  echo Project file not found: %PROJ%
  exit /b 1
)
echo Starting XCoinMonthChart using project:
echo   %PROJ%
dotnet run --project "%PROJ%" --urls http://localhost:5000
