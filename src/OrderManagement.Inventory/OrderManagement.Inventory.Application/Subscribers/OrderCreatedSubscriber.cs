using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderManagement.Inventory.Application.Events;
using OrderManagement.Inventory.Application.Services;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Products.API.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderManagement.Inventory.Application.Subscribers;

public class OrderCreatedSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderCreatedSubscriber> _logger;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string RoutingKey = "order.created";

    public OrderCreatedSubscriber(
        IServiceProvider serviceProvider,
        RabbitMqSettings settings,
        ILogger<OrderCreatedSubscriber> logger)
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

        _connection = await connectionFactory.CreateConnectionAsync("inventory-service-order-created-subscriber", stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(_settings.OrdersExchangeName, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(_settings.OrderCreatedQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(_settings.OrderCreatedQueueName, _settings.OrdersExchangeName, RoutingKey, cancellationToken: stoppingToken);

        _logger.LogInformation("OrderCreatedSubscriber initialized. Listening on queue: {Queue}", _settings.OrderCreatedQueueName);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var byteArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(byteArray);
                var orderEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(contentString);

                if (orderEvent == null)
                {
                    _logger.LogWarning("Received null or invalid OrderCreatedEvent message");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    return;
                }

                _logger.LogInformation("Received OrderCreatedEvent for OrderId: {OrderId} with {ItemCount} items",
                    orderEvent.OrderId,
                    orderEvent.Items.Count);

                using var scope = _serviceProvider.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                var validationResult = await inventoryService.ValidateOrderStockAsync(orderEvent);
                await eventPublisher.PublishAsync(validationResult);

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);

                _logger.LogInformation("Processed OrderCreatedEvent for OrderId: {OrderId}, Result: {Result}",
                    orderEvent.OrderId,
                    validationResult.IsApproved ? "APPROVED" : "REJECTED");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderCreatedEvent");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(_settings.OrderCreatedQueueName, autoAck: false, consumer, stoppingToken);
    }
    
}
