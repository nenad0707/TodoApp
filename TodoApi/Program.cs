using AspNetCoreRateLimit;
using TodoApi.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddStandardServices();
builder.AddAuthServices();
builder.AddHealthChecks();
builder.AddCustomServices();
builder.AddCorsServices();
builder.AddSerilogServices();
builder.AddRateLimitingService();
builder.AddPingServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    c.RoutePrefix = string.Empty; // default is "swagger"
    c.InjectStylesheet("/css/theme-material.css");
});

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseCors("AllowAllOrigins");

app.UseResponseCaching();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
