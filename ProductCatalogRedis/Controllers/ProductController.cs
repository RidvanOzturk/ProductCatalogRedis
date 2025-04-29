using Microsoft.AspNetCore.Mvc;
using ProductCatalogRedis.Models;
using ProductCatalogRedis.Services.Contracts;

namespace ProductCatalogRedis.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ProductResponseModel>> GetAll()
        => await productService.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseModel>> Get(int id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ProductResponseModel body)
    {
        var product = await productService.UpdateAsync(id, body);
        if (product == null)
        {
            return BadRequest();
        }
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteAsync(id);
        return NoContent();
    }
}
