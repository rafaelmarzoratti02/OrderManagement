using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderManagement.Inventory.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed;

    public RabbitMqEventPublisher(RabbitMqSettings settings)
    {
        _settings = settings;

        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = factory.CreateConnection("inventory-service-publisher");
        _channel = _connection.CreateChannel();

        _channel.ExchangeDeclare(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        _channel.ExchangeDeclare(
            exchange: _settings.OrdersExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));
        }

        var eventName = typeof(TEvent).Name;
        var routingKey = eventName.ToLower().Replace("event", "");
        var exchangeName = GetExchangeName(eventName);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    private string GetExchangeName(string eventName)
    {
        if (eventName.Contains("Order", StringComparison.OrdinalIgnoreCase))
        {
            return _settings.OrdersExchangeName;
        }

        return _settings.ExchangeName;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _channel?.Dispose();
        _connection?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
