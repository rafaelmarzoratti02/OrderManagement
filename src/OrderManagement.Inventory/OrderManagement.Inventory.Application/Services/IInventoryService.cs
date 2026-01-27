using OrderManagement.Inventory.Application.Events;
using OrderManagement.Inventory.Core.Events;

namespace OrderManagement.Inventory.Application.Services;

public interface IInventoryService
{
    Task<OrderInventoryValidated> ValidateOrderStockAsync(OrderCreatedEvent order);
    Task AddStockItem(ProductCreatedEvent product);
}
