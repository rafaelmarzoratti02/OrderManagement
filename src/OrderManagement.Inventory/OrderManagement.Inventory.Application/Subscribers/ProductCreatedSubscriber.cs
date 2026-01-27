using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderManagement.Inventory.Application.Events;
using OrderManagement.Inventory.Application.Services;
using OrderManagement.Products.API.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderManagement.Inventory.Application.Subscribers;

public class ProductCreatedSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProductCreatedSubscriber> _logger;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string Queue = "inventory-service/product-created";
    private const string RoutingKey = "productcreated";

    public ProductCreatedSubscriber(
        IServiceProvider serviceProvider,
        RabbitMqSettings settings,
        ILogger<ProductCreatedSubscriber> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = await connectionFactory.CreateConnectionAsync("inventory-service-product-created-subscriber", stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(_settings.ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(Queue, _settings.ExchangeName, RoutingKey, cancellationToken: stoppingToken);

        _logger.LogInformation("ProductCreatedSubscriber initialized. Listening on queue: {Queue}", Queue);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var byteArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(byteArray);
                var message = JsonConvert.DeserializeObject<ProductCreatedEvent>(contentString);

                if (message == null)
                {
                    _logger.LogWarning("Received null or invalid ProductCreatedEvent message");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

                await inventoryService.AddStockItem(message);

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ProductCreatedEvent");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(Queue, autoAck: false, consumer, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _channel?.Dispose();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        _connection?.Dispose();
        base.Dispose();
    }
}
