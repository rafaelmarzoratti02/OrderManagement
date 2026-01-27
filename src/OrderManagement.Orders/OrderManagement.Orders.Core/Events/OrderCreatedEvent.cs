namespace OrderManagement.Orders.Core.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public int OrderId { get; set; }
    public List<OrderItemEvent> Items { get; set; } = new();
}

public class OrderItemEvent
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
