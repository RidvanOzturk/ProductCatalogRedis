using StackExchange.Redis;

namespace ProductCatalogRedis.Caching;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;
    public RedisService(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }
    public IDatabase Db => _db;
}
