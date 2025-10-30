using System.Text.Json;
using System.Threading.RateLimiting;

namespace CodebridgeTestTask.Middleware;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddGlobalRateLimiting(this IServiceCollection services, int requestsPerSecond)
    {
        services.AddRateLimiter(o =>
        {
            o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>

                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "global",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = requestsPerSecond,
                        Window = TimeSpan.FromSeconds(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));
            o.OnRejected = async (context,ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers.RetryAfter = "1";
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(new { error = "Too many requests", retryAfterSeconds = 1 }), ct);
            };
        });
        return services;
    }
}