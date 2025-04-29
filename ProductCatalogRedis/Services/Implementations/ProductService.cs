// Services/Implementations/ProductService.cs
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProductCatalogRedis.Caching;
using ProductCatalogRedis.Data;
using ProductCatalogRedis.Models;
using ProductCatalogRedis.Services.Contracts;

namespace ProductCatalogRedis.Services.Implementations;

public class ProductService(IDistributedCache cache, ProductContext context, IOptions<CacheSettings> options) : IProductService
{
    private readonly TimeSpan _cacheDuration = options.Value.ProductCacheDuration;
    private const string KeyTemplate = "product-id-{0}";

    public async Task<IEnumerable<ProductResponseModel>> GetAllAsync()
        => await context.Products
                   .AsNoTracking()
                   .Select(p => new ProductResponseModel(p.Id, p.Name, p.Description, p.Price))
                   .ToListAsync();

    public async Task<ProductResponseModel> GetByIdAsync(int id)
    {
        var key = string.Format(KeyTemplate, id);
        var json = await cache.GetStringAsync(key);
        if (json is not null)
            return JsonSerializer.Deserialize<ProductResponseModel>(json)!;

        var e = await context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

        var model = new ProductResponseModel(e.Id, e.Name, e.Description, e.Price);

        await cache.SetStringAsync(key,
            JsonSerializer.Serialize(model),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

        return model;
    }

    public async Task<ProductResponseModel> UpdateAsync(int id, ProductResponseModel update)
    {
        var entity = await context.Products.FindAsync(id);
        entity.Name = update.Name;
        entity.Description = update.Description;
        entity.Price = update.Price;
        await context.SaveChangesAsync();
        await cache.RemoveAsync(string.Format(KeyTemplate, id));

        return update;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await context.Products.FindAsync(id);
        context.Products.Remove(entity);
        await context.SaveChangesAsync();
        await cache.RemoveAsync(string.Format(KeyTemplate, id));
    }
}
