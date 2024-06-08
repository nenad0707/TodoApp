using Microsoft.Extensions.Caching.Memory;

namespace TodoApi.Helpers;

/// <summary>
/// Helper class to ping a health endpoint and cache the health status.
/// </summary>
public class PingHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _healthEndpoint;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PingHelper> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingHelper"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory to create HTTP clients.</param>
    /// <param name="configuration">The configuration to retrieve the health endpoint URL.</param>
    /// <param name="cache">The memory cache to store the health status.</param>
    /// <param name="logger">The logger to log information and errors.</param>
    public PingHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache, ILogger<PingHelper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _healthEndpoint = configuration["ApiUrl"] + "/health";
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Pings the health endpoint and caches the result.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the health endpoint is healthy.</returns>
    public async Task<bool> PingHealth()
    {
        if (_cache.TryGetValue("HealthStatus", out bool isHealthy))
        {
            return isHealthy;
        }

        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            var response = await httpClient.GetAsync(_healthEndpoint);
            isHealthy = response.IsSuccessStatusCode;
            _cache.Set("HealthStatus", isHealthy, TimeSpan.FromMinutes(15));
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error pinging health endpoint: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Pings the health endpoint with retries if the initial attempt fails.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PingHealthWithRetry()
    {
        const int maxAttempts = 5;
        const int delay = 5000; // 5 seconds delay between retries

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (await PingHealth())
            {
                _logger.LogInformation("Health endpoint is up");
                return;
            }
            await Task.Delay(delay);
        }
        _logger.LogWarning("Health endpoint is down after multiple attempts");
    }
}