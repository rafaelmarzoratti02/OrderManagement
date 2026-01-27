using OrderManagement.Orders.Application.DTOs;
using OrderManagement.Orders.Application.Models;
using OrderManagement.Orders.Core.Events;

namespace OrderManagement.Orders.Application.Services;

public interface IOrderService
{
    Task<int> CreateOrder(CreateOrderInputModel inputModel);
    Task<OrderViewModel?> GetOrderById(int orderId);
    Task<List<OrderViewModel>> GetAllOrders();
    Task UpdateOrderStatusAsync(OrderInventoryValidatedEvent validationResult);
}
