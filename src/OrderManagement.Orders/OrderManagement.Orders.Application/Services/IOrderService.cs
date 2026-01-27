using OrderManagement.Orders.Application.DTOs;
using OrderManagement.Orders.Application.Models;
using OrderManagement.Orders.Core.Events;

namespace OrderManagement.Orders.Application.Services;

public interface IOrderService
{
    Task<int> CreateOrderAsync(CreateOrderInputModel inputModel);
    Task<OrderViewModel?> GetOrderByIdAsync(int orderId);
    Task<List<OrderViewModel>> GetAllOrdersAsync();
    Task UpdateOrderStatusAsync(OrderInventoryValidatedEvent validationResult);
}
