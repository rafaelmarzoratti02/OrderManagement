using Microsoft.AspNetCore.Mvc;
using OrderManagement.Inventory.Application.Models;
using OrderManagement.Inventory.Application.Services;

namespace OrderManagement.Inventory.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPut("{productSku}")]
    public async Task<IActionResult> Put(UpdateStockItemInputModel model)
    {
        await _inventoryService.UpdateStockItem(model);
        return NoContent();
    }
    [HttpGet("{sku}")]
    public async Task<IActionResult> GetBySku(string sku)
    {
        var model = await _inventoryService.GetStockItemBySku(sku);

        if (model is null)
        {
            return NotFound();
        }

        return Ok(model);
    }
    
}