namespace OrderManagement.Inventory.Infrastructure.Messaging;

public interface IEventPublisher
{

    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;

}