using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderManagement.Orders.Application.Services;
using OrderManagement.Orders.Core.Events;
using OrderManagement.Orders.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderManagement.Orders.Application.Subscribers;

public class OrderValidationResultSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderValidationResultSubscriber> _logger;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string RoutingKey = "orderinventoryvalidated";

    public OrderValidationResultSubscriber(
        IServiceProvider serviceProvider,
        RabbitMqSettings settings,
        ILogger<OrderValidationResultSubscriber> logger)
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

        _connection = await connectionFactory.CreateConnectionAsync("orders-service-validation-subscriber", stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(_settings.ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(_settings.ValidationResultQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(_settings.ValidationResultQueueName, _settings.ExchangeName, RoutingKey, cancellationToken: stoppingToken);

        _logger.LogInformation("OrderValidationResultSubscriber initialized. Listening on queue: {Queue}", _settings.ValidationResultQueueName);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var byteArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(byteArray);

                _logger.LogInformation("[Subscriber] Received message from queue: {Queue}", _settings.ValidationResultQueueName);
                _logger.LogInformation("[Subscriber] Raw message: {Message}", contentString);

                var validationEvent = JsonConvert.DeserializeObject<OrderInventoryValidatedEvent>(contentString);

                if (validationEvent == null)
                {
                    _logger.LogWarning("[Subscriber] Received null or invalid OrderInventoryValidatedEvent message");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    return;
                }

                _logger.LogInformation("[Subscriber] Parsed event - OrderId: {OrderId}, IsApproved: {IsApproved}, Reason: {Reason}",
                    validationEvent.OrderId,
                    validationEvent.IsApproved,
                    validationEvent.Reason ?? "N/A");

                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                await orderService.UpdateOrderStatusAsync(validationEvent);

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);

                _logger.LogInformation("[Subscriber] Successfully processed OrderInventoryValidatedEvent for OrderId: {OrderId}", validationEvent.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Subscriber] Error processing OrderInventoryValidatedEvent");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(_settings.ValidationResultQueueName, autoAck: false, consumer, stoppingToken);
    }
}
