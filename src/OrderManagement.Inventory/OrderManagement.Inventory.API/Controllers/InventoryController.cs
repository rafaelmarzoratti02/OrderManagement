using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Inventory.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : Controller
{
    [HttpPut("{productSku}")]
    public async Task<IActionResult> Put(string productSku)
    {
        return Ok();
    }
}