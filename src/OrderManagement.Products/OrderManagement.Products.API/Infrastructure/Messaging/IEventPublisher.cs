namespace OrderManagement.Products.API.Infrastructure.Messaging;

public interface IEventPublisher
{

    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;

}