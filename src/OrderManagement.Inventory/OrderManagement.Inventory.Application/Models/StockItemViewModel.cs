using OrderManagement.Inventory.Core.Entities;

namespace OrderManagement.Inventory.Application.Models;

public class StockItemViewModel
{
    public StockItemViewModel(int id, string title,string sku, int quantity)
    {
        Id = id;
        Title = title;
        Sku = sku;
        Quantity = quantity;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Sku { get; set; } 
    public int Quantity { get; set; }
    
    public static StockItemViewModel FromEntity(StockItem entity)
        => new StockItemViewModel(entity.Id,entity.Title,entity.Sku,entity.Quantity);
}