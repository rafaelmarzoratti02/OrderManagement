using System.Text;
using System.Text.Json;
using OrderManagement.Products.API.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace OrderManagement.Inventory.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqEventPublisher(RabbitMqSettings settings)
    {
        _settings = settings;

        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;

        // Declare both exchanges
        _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        ).GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: _settings.OrdersExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        ).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        var eventName = typeof(TEvent).Name;
        var routingKey = eventName.ToLower().Replace("event", "");
        var exchangeName = GetExchangeName(eventName);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            body: body);

        Console.WriteLine($"[Publisher] Published {eventName} to exchange {exchangeName} with routing key {routingKey}");
    }

    private string GetExchangeName(string eventName)
    {
        // OrderInventoryValidated should be published to Orders exchange
        if (eventName.Contains("Order", StringComparison.OrdinalIgnoreCase))
        {
            return _settings.OrdersExchangeName;
        }

        return _settings.ExchangeName;
    }
}
