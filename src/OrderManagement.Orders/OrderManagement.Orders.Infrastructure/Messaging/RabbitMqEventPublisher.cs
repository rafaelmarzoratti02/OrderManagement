using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderManagement.Orders.Infrastructure.Messaging;

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

        _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        ).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        var eventName = typeof(TEvent).Name;
        var routingKey = GetRoutingKey(eventName);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: _settings.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        Console.WriteLine($"[Publisher] Published {eventName} to exchange {_settings.ExchangeName} with routing key {routingKey}");
    }

    private static string GetRoutingKey(string eventName)
    {
        // OrderCreatedEvent -> order.created
        if (eventName.Equals("OrderCreatedEvent", StringComparison.OrdinalIgnoreCase))
        {
            return "order.created";
        }

        // Default behavior: remove "Event" suffix and lowercase
        return eventName.ToLower().Replace("event", "");
    }
}
