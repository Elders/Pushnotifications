using Cassandra;
using Cassandra.Lock;
using Elders.Cronus.AtomicAction;
using Elders.Cronus.AtomicAction.InMemory;
using Elders.Pandora;
using PushNotifications.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Multitenancy.Tracker
{
    public class BadgeCountTrackerFactory : IBadgeCountTrackerFactory
    {
        private readonly ConcurrentDictionary<string, IBadgeCountTracker> _store;

        private readonly Pandora _pandora;

        public BadgeCountTrackerFactory(Pandora pandora)
        {
            if (ReferenceEquals(null, pandora)) throw new ArgumentNullException(nameof(pandora));

            _store = new ConcurrentDictionary<string, IBadgeCountTracker>();
            _pandora = pandora;
        }

        public IBadgeCountTracker GetService(string tenant)
        {
            if (string.IsNullOrEmpty(tenant)) throw new ArgumentNullException(nameof(tenant));

            if (_store.ContainsKey(tenant) == false)
                Initialize(tenant);

            IBadgeCountTracker service;
            _store.TryGetValue(tenant, out service);

            return service;
        }

        void Initialize(string tenant)
        {
            string connectionString;

            if (_pandora.TryGet($"{tenant}_cassandra_badge_counter_connection_string", out connectionString))
            {
                ISession session = SessionCreator.Create(connectionString);

                bool useRedis;
                _pandora.TryGet("use_redis_lock", out useRedis);

                if (useRedis)
                {
                    var redisConnectionString = _pandora.Get("redis_connection_string");
                    ILock @lock = RedisLockFactory.CreateInstance(redisConnectionString);

                    var service = new BadgeCountTracker(session, @lock);
                    _store.AddOrUpdate(tenant, service, (key, oldValue) => service);
                }
                else
                {
                    ILock @lock = new InMemoryLock();

                    var service = new BadgeCountTracker(session, @lock);
                    _store.AddOrUpdate(tenant, service, (key, oldValue) => service);
                }
            }
        }

        private class SessionCreator
        {
            public static ISession Create(string connectionString, Dictionary<string, string> replication = null, bool durableWrites = true)
            {
                var cluster = Cluster.Builder()
                    .WithConnectionString(connectionString)
                    .Build();

                var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists(replication, durableWrites);
                return session;
            }
        }
    }
}
