namespace OrderManagement.Orders.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
}
