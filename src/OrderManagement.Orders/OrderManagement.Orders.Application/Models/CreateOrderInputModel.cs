namespace OrderManagement.Orders.Application.DTOs;

public class CreateOrderInputModel
{
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
