using Hangfire;
using TodoApi.Helpers;

namespace TodoApi.StartupConfig;

/// <summary>
/// Provides extension methods for configuring Hangfire jobs in the application.
/// </summary>
public static class HangfireMiddlewareExtensions
{
    /// <summary>
    /// Configures Hangfire jobs for the application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        var serviceProvider = app.ApplicationServices;
        var pingHelper = serviceProvider.GetRequiredService<PingHelper>();

        RecurringJob.AddOrUpdate(
           "PingApi",
           () => pingHelper.PingApiWithRetry(),
           "*/15 * * * *"); // Adjust to "*/5 * * * *" if needed

        RecurringJob.AddOrUpdate(
            "PingDatabase",
            () => pingHelper.PingDatabaseWithRetry(),
            "*/15 * * * *"); // Adjust to "*/5 * * * *" if needed
        return app;
    }
}
