namespace OrderManagement.Products.API.Entities.Events;

public class ProductCreatedEvent : IDomainEvent
{
    public ProductCreatedEvent(string sku, int quantity, string title)
    {
        Sku = sku;
        Quantity = quantity;
        Title = title;
    }

    public string Sku { get; private set; }
    public int Quantity { get; set; }

    public string Title { get; set; }
}