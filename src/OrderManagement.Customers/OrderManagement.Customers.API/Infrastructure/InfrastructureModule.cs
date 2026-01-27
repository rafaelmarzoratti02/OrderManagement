using OrderManagement.Customers.API.Infrastructure.Persistence;

namespace OrderManagement.Customers.API.Infrastructure.Extensions;

public static class InfrastructureModule
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDbSettings not configured");

        services.AddSingleton(settings);
        services.AddSingleton<MongoDbContext>();

        return services;
    }
}
