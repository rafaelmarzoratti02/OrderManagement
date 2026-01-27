using OrderManagement.Inventory.Application.Events;
using OrderManagement.Inventory.Application.Models;
using OrderManagement.Inventory.Core.Events;

namespace OrderManagement.Inventory.Application.Services;

public interface IInventoryService
{
    Task ValidateOrderStock(OrderCreatedEvent order);
    Task AddStockItem(ProductCreatedEvent product);
    Task UpdateStockItem(UpdateStockItemInputModel  updateStockItemInputModel);
    Task<StockItemViewModel> GetStockItemBySku(string sku);
}
