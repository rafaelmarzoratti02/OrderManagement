using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderManagement.Products.API.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace OrderManagement.Products.API.HealthChecks;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqHealthCheck(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            using var connection = factory.CreateConnection("products-healthcheck");
            using var channel = connection.CreateChannel();

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ is reachable."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ is not reachable.", ex));
        }
    }
}

