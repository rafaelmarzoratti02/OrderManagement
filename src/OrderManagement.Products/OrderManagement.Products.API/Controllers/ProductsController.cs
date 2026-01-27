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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var model = await _productService.GetProductById(id);
        return Ok(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(CreateProductInputModel model)
    {
        var id = await _productService.CreateProduct(model);
        return Created();
    }
}