using ProductCatalogRedis.Models;

namespace ProductCatalogRedis.Services.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductResponseModel>> GetAllAsync();
    Task<ProductResponseModel> GetByIdAsync(int id);
    Task<ProductResponseModel> UpdateAsync(int id, ProductResponseModel update);
    Task DeleteAsync(int id);
}
