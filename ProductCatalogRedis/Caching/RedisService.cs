using StackExchange.Redis;

namespace ProductCatalogRedis.Caching;
public class RedisService(IConnectionMultiplexer multiplexer) : IRedisService
{
    public IDatabase Db => multiplexer.GetDatabase();
}
