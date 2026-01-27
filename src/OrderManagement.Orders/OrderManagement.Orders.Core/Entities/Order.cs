using OrderManagement.Orders.Core.Enums;

namespace OrderManagement.Orders.Core.Entities;

public class Order : BaseEntity
{
    public Order()
    {
        Status = OrderStatus.PENDING;
        Items = new List<OrderItem>();
    }

    public OrderStatus Status { get; set; }
    public string? ValidationReason { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; }

    public void Approve()
    {
        Status = OrderStatus.APPROVED;
        UpdatedAt = DateTime.Now;
    }

    public void RejectDueToStock(string reason)
    {
        Status = OrderStatus.OUT_OF_STOCK;
        ValidationReason = reason;
        UpdatedAt = DateTime.Now;
    }
}
