using Microsoft.AspNetCore.Mvc;
using OrderManagement.Customers.API.Application.Models;
using OrderManagement.Customers.API.Application.Services;

namespace OrderManagement.Customers.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var model = await _customerService.GetCustomerById(id);
        return Ok(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(CreateCustomerInputModel model)
    {
        var id = await _customerService.CreateCustomer(model);
        return Created();
    }
    
}