using Hangfire;
using TodoApi.Helpers;

namespace TodoApi.StartupConfig;

/// <summary>
/// Extension methods for configuring Hangfire recurring jobs in the application.
/// </summary>
public static class HangfireMiddlewareExtensions
{
    /// <summary>
    /// Configures Hangfire recurring jobs for the application.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <returns>The application builder instance with Hangfire jobs configured.</returns>
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;

        // Adds or updates a recurring job to ping an API every 15 minutes.
        RecurringJob.AddOrUpdate(
            "PingApi",
            () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PingHelper>().PingApiWithRetry(),
            "*/15 * * * *"); // Adjust to "*/5 * * * *" if needed

        return app;
    }
}