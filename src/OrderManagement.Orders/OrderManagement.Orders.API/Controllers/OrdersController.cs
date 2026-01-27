using Microsoft.AspNetCore.Mvc;
using OrderManagement.Orders.Application.DTOs;
using OrderManagement.Orders.Application.Models;
using OrderManagement.Orders.Application.Services;

namespace OrderManagement.Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrderInputModel inputModel)
    {
        if (inputModel.Items == null || inputModel.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one item");
        }

        var order = await _orderService.CreateOrderAsync(inputModel);

        return CreatedAtAction(nameof(GetOrderById), new { id = order }, inputModel);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound($"Order with id {id} not found");
        }

        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }
}
