namespace OrderManagement.Inventory.Application.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public List<OrderItemEvent> Items { get; set; } = new();
}

public class OrderItemEvent
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
