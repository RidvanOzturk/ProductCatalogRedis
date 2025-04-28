using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogRedis.Caching;
using ProductCatalogRedis.Data;
using ProductCatalogRedis.Models;
using ProductCatalogRedis.Services.Contracts;
using System.Text.Json;

namespace ProductCatalogRedis.Services.Implementations;

public class ProductService(ProductContext productContext, IRedisService redisService, IOptions<CacheSettings> options) : IProductService
{
    private const string ProductCacheKeyTemplate = "product-id-{0}";



    public async Task<IEnumerable<ProductResponseModel>> GetAllAsync()
    {
        return await productContext.Products
            .AsNoTracking()
            .Select(p => new ProductResponseModel(p.Id, p.Name, p.Description, p.Price))
            .ToListAsync();
    }

    public async Task<ProductResponseModel> GetByIdAsync(int id)
    {
        var key = string.Format(ProductCacheKeyTemplate, id);
        TimeSpan cacheDuration = options.Value.ProductCacheDuration;
          var cached = await redisService.Db.StringGetAsync(key);
        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<ProductResponseModel>(cached)!;
        }

        var entity = await productContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        var model = new ProductResponseModel(entity.Id, entity.Name, entity.Description, entity.Price);

        await redisService.Db.StringSetAsync(
            key,
            JsonSerializer.Serialize(model),
            cacheDuration
        );

        return model;
    }
}