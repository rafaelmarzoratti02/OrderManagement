using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Inventory.Application.Events;
using OrderManagement.Inventory.Application.Models;
using OrderManagement.Inventory.Core.Entities;
using OrderManagement.Inventory.Core.Events;
using OrderManagement.Inventory.Infrastructure.Messaging;
using OrderManagement.Inventory.Infrastructure.Persistence;

namespace OrderManagement.Inventory.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(InventoryDbContext dbContext, IEventPublisher eventPublisher, ILogger<InventoryService> logger)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task AddStockItem(ProductCreatedEvent product)
    {
        var existingItem = await _dbContext.StockItems
            .FirstOrDefaultAsync(s => s.Sku == product.Sku);

        if (existingItem != null)
        {
            existingItem.Quantity += product.Quantity;
        }
        else
        {
            var stockItem = new StockItem(product.Sku, product.Title, product.Quantity);
            await _dbContext.StockItems.AddAsync(stockItem);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateStockItem(UpdateStockItemInputModel updateStockItemInputModel)
    {
        var stockItem = await _dbContext.StockItems.FirstOrDefaultAsync(s => s.Sku == updateStockItemInputModel.Sku);
        if(stockItem == null)
            throw new Exception($"Sku '{updateStockItemInputModel.Sku}' not found");
        
        stockItem.UpdateQuantity(updateStockItemInputModel.Quantity);
        
        await _dbContext.SaveChangesAsync();
    }

    public async  Task<StockItemViewModel> GetStockItemBySku(string sku)
    {
        var stockItem = await _dbContext.StockItems.FirstOrDefaultAsync(s => s.Sku == sku);
        if(stockItem == null)
            throw new Exception($"Sku '{sku}' not found");
        
        var model = StockItemViewModel.FromEntity(stockItem);
        
        return  model;
    }

    public async Task ValidateOrderStock(OrderCreatedEvent order)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var skus = order.Items.Select(i => i.Sku).ToList();

            var stockItems = await _dbContext.StockItems
                .Where(s => skus.Contains(s.Sku))
                .ToListAsync();

            var unavailableItems = new List<string>();

            foreach (var orderItem in order.Items)
            {
                var stockItem = stockItems.FirstOrDefault(s => s.Sku == orderItem.Sku);

                if (stockItem == null)
                {
                    unavailableItems.Add($"SKU '{orderItem.Sku}' not found");
                    continue;
                }

                if (stockItem.Quantity < orderItem.Quantity)
                {
                    unavailableItems.Add($"SKU '{orderItem.Sku}' insufficient stock (requested: {orderItem.Quantity}, available: {stockItem.Quantity})");
                }
            }

            var isApproved = unavailableItems.Count == 0;

            if (isApproved)
            {
                foreach (var orderItem in order.Items)
                {
                    var stockItem = stockItems.First(s => s.Sku == orderItem.Sku);
                    stockItem.DecrementQuantity(orderItem.Quantity);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Stock decremented for OrderId: {OrderId}", order.OrderId);
            }
            else
            {
                await transaction.RollbackAsync();
            }

            var @event = new OrderInventoryValidated
            {
                OrderId = order.OrderId,
                IsApproved = isApproved,
                Reason = isApproved ? null : string.Join("; ", unavailableItems)
            };

            await _eventPublisher.PublishAsync(@event);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error validating order stock for OrderId: {OrderId}", order.OrderId);
            throw;
        }
    }
}
