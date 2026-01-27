namespace OrderManagement.Inventory.Application.Models;

public class UpdateStockItemInputModel
{
    public string Sku { get; set; }
    public int Quantity { get; set; }
}