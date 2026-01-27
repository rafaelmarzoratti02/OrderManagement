namespace OrderManagement.Inventory.Application.Models;

public class UpdateStockItemInputModel
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}