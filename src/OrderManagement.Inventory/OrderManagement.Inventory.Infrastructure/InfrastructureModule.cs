using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Inventory.Infrastructure.Persistence;

namespace OrderManagement.Inventory.Infrastructure;

public static class InfrastructureModule
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddData(configuration)
            .AddRabbitMq(configuration);


        return services;
    }
    
    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()!;
        services.AddSingleton(rabbitMqSettings);
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        
        return services;
    }
    
    private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryDb");

        services.AddDbContext<InventoryDbContext>(o => o.UseSqlServer(connectionString));

        return services;
    }

}