namespace OrderManagement.Inventory.Application.Events;

public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
