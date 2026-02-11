using Microsoft.AspNetCore.Mvc;
using OrderManagement.Products.API.Application.Models;
using OrderManagement.Products.API.Application.Services;

namespace OrderManagement.Products.API.Controllers;


[ApiController]
[Route("api/products")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var model = await _productService.GetAll();
        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var model = await _productService.GetProductById(id);

        if (model is null)
        {
            return NotFound();
        }

        return Ok(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(CreateProductInputModel model)
    {
        var id = await _productService.CreateProduct(model);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
}