using Dapper;
using System.Data.SqlClient;

namespace TodoApi.Helpers;

/// <summary>
/// Provides methods to ping the API and database to check their availability.
/// </summary>
public class PingHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _connectionString;
    private readonly string _apiUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingHelper"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The factory to create HTTP clients.</param>
    /// <param name="configuration">The configuration to retrieve the connection string.</param>
    public PingHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _connectionString = configuration.GetConnectionString("Default")!;
        _apiUrl = configuration["ApiUrl"]!;
    }

    /// <summary>
    /// Pings the API to check if it is up and running.
    /// </summary>
    /// <returns>True if the API is up, otherwise false.</returns>
    public async Task<bool> PingApi()
    {
        var httpClient = _httpClientFactory.CreateClient("api");
        httpClient.BaseAddress = new Uri(_apiUrl);
        try
        {
            var response = await httpClient.GetAsync("health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pinging API: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Pings the database to check if it is up and running.
    /// </summary>
    /// <returns>True if the database is up, otherwise false.</returns>
    public async Task<bool> PingDatabase()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>("SELECT 1");
                return result == 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pinging database: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Tries to ping the API with retries in case of failures.
    /// </summary>
    public async Task PingApiWithRetry()
    {
        const int maxAttempts = 5;
        const int delay = 5000; // 5 seconds delay between retries

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (await PingApi())
            {
                Console.WriteLine("API is up");
                return;
            }
            await Task.Delay(delay);
        }
        Console.WriteLine("API is down after multiple attempts");
    }

    /// <summary>
    /// Tries to ping the database with retries in case of failures.
    /// </summary>
    public async Task PingDatabaseWithRetry()
    {
        const int maxAttempts = 5;
        const int delay = 5000; // 5 seconds delay between retries

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (await PingDatabase())
            {
                Console.WriteLine("Database is up");
                return;
            }
            await Task.Delay(delay);
        }
        Console.WriteLine("Database is down after multiple attempts");
    }
}
