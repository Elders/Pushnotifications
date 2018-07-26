using Elders.Cronus.AtomicAction;
using RedLock;
using System;

namespace Multitenancy.Tracker
{
    public class RedisLock : ILock
    {
        private readonly IRedisLockManager redisLockManager;

        public RedisLock(IRedisLockManager redisLockManager)
        {
            if (ReferenceEquals(null, redisLockManager)) throw new ArgumentNullException(nameof(redisLockManager));

            this.redisLockManager = redisLockManager;
        }

        public bool IsLocked(string resource)
        {
            if (string.IsNullOrEmpty(resource)) throw new ArgumentNullException(nameof(resource));

            return redisLockManager.IsLocked(resource);
        }

        public bool Lock(string resource, TimeSpan ttl)
        {
            if (string.IsNullOrEmpty(resource)) throw new ArgumentNullException(nameof(resource));

            return redisLockManager.Lock(resource, ttl);
        }

        public void Unlock(string resource)
        {
            if (string.IsNullOrEmpty(resource)) throw new ArgumentNullException(nameof(resource));

            redisLockManager.Unlock(resource);
        }
    }
}

