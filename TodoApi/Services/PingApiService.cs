using TodoApi.Helpers;

namespace TodoApi.Services;

/// <summary>
/// A service that pings an API at regular intervals to keep the server and database awake.
/// Implements the <see cref="IHostedService"/> and <see cref="IDisposable"/> interfaces.
/// </summary>
public class PingApiService : IHostedService, IDisposable
{
    private Timer? _timer; // Make the _timer field nullable
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingApiService"/> class.
    /// </summary>
    /// <param name="scopeFactory">The factory to create service scopes.</param>
    public PingApiService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Starts the service and begins the timer to ping the API at regular intervals.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15)); // Adjust the interval as needed
        return Task.CompletedTask;
    }

    /// <summary>
    /// The method that is called by the timer to perform the work of pinging the API.
    /// </summary>
    /// <param name="state">An object containing application-specific information relevant to the method.</param>
    private void DoWork(object? state) // Make the state parameter nullable
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var pingHelper = scope.ServiceProvider.GetRequiredService<PingHelper>();
            pingHelper.PingHealthWithRetry().Wait(); // Ping the health endpoint to keep the server and database awake
        }
    }

    /// <summary>
    /// Stops the service and disables the timer.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes the timer.
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }
}