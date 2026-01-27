namespace OrderManagement.Orders.Application.Models;

public class OrderViewModel
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ValidationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderItemResponse
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
