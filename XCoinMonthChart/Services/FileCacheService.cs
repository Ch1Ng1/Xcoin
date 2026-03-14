using Microsoft.Extensions.Logging;

namespace XCoinMonthChart.Services;

/// <summary>
/// Manages file-based JSON caching with configurable time-to-live.
/// Each entry is identified by a key that maps to a file on disk.
/// </summary>
public class FileCacheService
{
    private readonly string _cacheDirectory;
    private readonly ILogger<FileCacheService> _logger;
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);

    /// <param name="logger">Logger instance for diagnostics.</param>
    public FileCacheService(ILogger<FileCacheService> logger)
    {
        _logger = logger;
        _cacheDirectory = Path.Combine(AppContext.BaseDirectory, "cache");
        Directory.CreateDirectory(_cacheDirectory);
    }

    /// <summary>
    /// Attempts to read a cached JSON string for the given <paramref name="key"/>.
    /// Returns <c>null</c> if the cache is missing, stale, or unreadable.
    /// </summary>
    /// <param name="key">Logical cache key (used as the file name, e.g. "bitcoin-2026").</param>
    /// <param name="ttl">Optional custom time-to-live. Defaults to 24 hours.</param>
    public async Task<string?> TryReadAsync(string key, TimeSpan? ttl = null)
    {
        var filePath = GetFilePath(key);
        var maxAge = ttl ?? DefaultTtl;

        if (!File.Exists(filePath))
            return null;

        var lastWrite = File.GetLastWriteTimeUtc(filePath);
        if (DateTime.UtcNow - lastWrite > maxAge)
        {
            _logger.LogDebug("Cache expired for key '{Key}' (age {Age})", key, DateTime.UtcNow - lastWrite);
            return null;
        }

        try
        {
            var content = await File.ReadAllTextAsync(filePath);
            _logger.LogDebug("Cache hit for key '{Key}'", key);
            return string.IsNullOrWhiteSpace(content) ? null : content;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read cache file for key '{Key}'", key);
            return null;
        }
    }

    /// <summary>
    /// Writes <paramref name="json"/> to the cache under the given <paramref name="key"/>.
    /// Failures are logged but do not throw.
    /// </summary>
    public async Task WriteAsync(string key, string json)
    {
        var filePath = GetFilePath(key);
        try
        {
            await File.WriteAllTextAsync(filePath, json);
            _logger.LogDebug("Cache written for key '{Key}'", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write cache file for key '{Key}'", key);
        }
    }

    private string GetFilePath(string key) => Path.Combine(_cacheDirectory, $"{key}.json");
}
