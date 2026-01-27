using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Messaging;

namespace OrderManagement.Inventory.Infrastructure;

public static class InfrastructureModule
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        


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