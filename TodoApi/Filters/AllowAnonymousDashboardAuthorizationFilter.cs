using Hangfire.Dashboard;

namespace TodoApi.Filters;

/// <summary>
/// An authorization filter that allows all users to access the Hangfire dashboard.
/// </summary>
public class AllowAnonymousDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    /// Authorizes access to the Hangfire dashboard.
    /// </summary>
    /// <param name="context">The dashboard context.</param>
    /// <returns>Always returns true, allowing all users to access the dashboard.</returns>
    public bool Authorize(DashboardContext context)
    {
        return true; // Allow all users to access the dashboard
    }
}