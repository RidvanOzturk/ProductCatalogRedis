using StackExchange.Redis;

namespace ProductCatalogRedis.Caching;

public interface IRedisService
{
    IDatabase Db { get; }
}