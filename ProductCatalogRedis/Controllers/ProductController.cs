using Microsoft.AspNetCore.Mvc;
using ProductCatalogRedis.Models;
using ProductCatalogRedis.Services.Contracts;

namespace ProductCatalogRedis.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseModel>>> GetAll()
    {
        var list = await productService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseModel>> Get(int id)
    {
        var dto = await productService.GetByIdAsync(id);
        return Ok(dto);

    }
}
