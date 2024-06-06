using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Helpers;

namespace TodoApi.StartupConfig
{
    public static class HangfireMiddlewareExtensions
    {
        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            RecurringJob.AddOrUpdate(
                "PingApi",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PingHelper>().PingApiWithRetry(),
                "*/15 * * * *"); // Adjust to "*/5 * * * *" if needed

            RecurringJob.AddOrUpdate(
                "PingDatabase",
                () => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PingHelper>().PingDatabaseWithRetry(),
                "*/15 * * * *"); // Adjust to "*/5 * * * *" if needed

            return app;
        }
    }
}
