using Elders.Cronus.AtomicAction;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Pandora;
using RedLock;
using System;

namespace Cassandra.Lock
{
    public static class CassandraProjectionsStoreSettingsExtensions
    {
        public static T UseLocking<T>(this T self, Pandora pandora, TimeSpan? ttl = null) where T : ICassandraProjectionsStoreSettings
        {
            bool useRedis;
            if (pandora.TryGet("use_redis_lock", out useRedis) == false)
            {
                return self;
            }

            if (useRedis)
            {
                var connectionString = pandora.Get("redis_connection_string");
                var redlock = new RedisLockManager(connectionString);
                self.UseLock(new RedisLock(redlock), ttl);
            }

            return self;
        }
    }

    public class RedisLockFactory
    {
        public static ILock CreateInstance(string connectionString)
        {
            var redlock = new RedisLockManager(connectionString);
            return new RedisLock(redlock);
        }
    }
}
