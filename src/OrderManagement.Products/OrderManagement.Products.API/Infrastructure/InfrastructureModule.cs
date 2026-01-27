using OrderManagement.Products.API.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Persistence;

namespace OrderManagement.Products.API.Infrastructure;


public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration)
            .AddRabbitMq(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
                       ?? throw new InvalidOperationException("MongoDbSettings not configured");

        services.AddSingleton(settings);
        services.AddSingleton<MongoDbContext>();

        return services;
    }
    
    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()!;
        services.AddSingleton(rabbitMqSettings);
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        
        return services;
    }
    


}