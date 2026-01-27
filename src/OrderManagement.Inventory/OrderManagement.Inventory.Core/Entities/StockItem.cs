namespace OrderManagement.Inventory.Core.Entities;

public class StockItem : BaseEntity
{
    public StockItem(string sku, string title, int quantity)
    {
        Sku = sku;
        Title = title;
        Quantity = quantity;
    }
    public string Sku { get; private set; }
    public string Title { get; private set; }
    public int Quantity { get; set; }
}