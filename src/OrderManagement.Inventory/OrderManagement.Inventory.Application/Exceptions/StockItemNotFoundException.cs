namespace OrderManagement.Inventory.Application.Exceptions;

public sealed class StockItemNotFoundException : Exception
{
    public string Sku { get; }

    public StockItemNotFoundException(string sku)
        : base($"Stock item with SKU '{sku}' was not found.")
    {
        Sku = sku;
    }
}

