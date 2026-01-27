using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Orders.Application.DTOs;
using OrderManagement.Orders.Application.Models;
using OrderManagement.Orders.Core.Entities;
using OrderManagement.Orders.Core.Events;
using OrderManagement.Orders.Infrastructure.Messaging;
using OrderManagement.Orders.Infrastructure.Persistence;

namespace OrderManagement.Orders.Application.Services;

public class OrderService : IOrderService
{
    private readonly OrdersDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrdersDbContext dbContext,
        IEventPublisher eventPublisher,
        ILogger<OrderService> logger)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<int> CreateOrder(CreateOrderInputModel inputModel)
    {
        var order = new Order();

        foreach (var item in inputModel.Items)
        {
            order.Items.Add(new OrderItem(item.Sku, item.Quantity));
        }

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            Items = order.Items.Select(i => new OrderItemEvent
            {
                Sku = i.Sku,
                Quantity = i.Quantity
            }).ToList()
        };

        await _eventPublisher.PublishAsync(orderCreatedEvent);

        _logger.LogInformation("OrderCreatedEvent published for OrderId: {OrderId}", order.Id);

        return order.Id;
    }

    public async Task<OrderViewModel?> GetOrderById(int orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found with Id: {OrderId}", orderId);
            return null;
        }

        return MapToResponse(order);
    }

    public async Task<List<OrderViewModel>> GetAllOrders()
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task UpdateOrderStatusAsync(OrderInventoryValidatedEvent validationResult)
    {
        _logger.LogInformation("OrderId: {OrderId}", validationResult.OrderId);
        _logger.LogInformation("IsApproved: {IsApproved}", validationResult.IsApproved);

        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == validationResult.OrderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found for validation update. OrderId: {OrderId}", validationResult.OrderId);
            return;
        }

        _logger.LogInformation("Order Items:");
        foreach (var item in order.Items)
        {
            _logger.LogInformation("  - SKU: {Sku}, Quantity: {Quantity}", item.Sku, item.Quantity);
        }

        if (validationResult.IsApproved)
        {
            order.Approve();
            _logger.LogInformation("Status: APPROVED");
            _logger.LogInformation("All items are available in stock.");
        }
        else
        {
            order.RejectDueToStock(validationResult.Reason ?? "Unknown reason");
            _logger.LogWarning("Status: OUT_OF_STOCK");
            _logger.LogWarning("Items with validation errors:");

            var reasons = validationResult.Reason?.Split("; ") ?? Array.Empty<string>();
            foreach (var reason in reasons)
            {
                _logger.LogWarning("  - {Reason}", reason);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private static OrderViewModel MapToResponse(Order order)
    {
        return new OrderViewModel
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            ValidationReason = order.ValidationReason,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Sku = i.Sku,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
