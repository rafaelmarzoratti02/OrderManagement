namespace OrderManagement.Orders.Core.Entities;

public class OrderItem : BaseEntity
{
    public OrderItem() { }

    public OrderItem(string sku, int quantity)
    {
        Sku = sku;
        Quantity = quantity;
    }

    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
