using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using OrderManagement.Products.API.Infrastructure.Persistence;

namespace OrderManagement.Products.API.HealthChecks;

public sealed class MongoDbHealthCheck : IHealthCheck
{
    private readonly MongoDbContext _context;

    public MongoDbHealthCheck(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Run a lightweight command against the database to verify connectivity.
            await _context.Products.Database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }", cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is not reachable.", ex);
        }
    }
}

