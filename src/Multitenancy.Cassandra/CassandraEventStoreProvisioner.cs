using System.Collections.Generic;
using DataStaxCassandra = Cassandra;
using Elders.Cronus.IocContainer;
using Multitenancy.EventStore;

namespace Multitenancy.Cassandra.EventStore
{
    public class CassandraEventStoreProvisioner : IEventStoreProvisioner
    {
        readonly Dictionary<string, Elders.Cronus.EventStore.IEventStore> tenantStores;

        readonly Dictionary<string, Elders.Cronus.EventStore.IEventStorePlayer> tenantPlayers;

        readonly Elders.Cronus.Pipeline.Config.ISettingsBuilder builder;

        readonly Elders.Cronus.Persistence.Cassandra.Config.ICassandraEventStoreSettings settings;

        public CassandraEventStoreProvisioner(Elders.Cronus.Pipeline.Config.ISettingsBuilder builder, Elders.Cronus.Persistence.Cassandra.Config.ICassandraEventStoreSettings settings)
        {
            this.settings = settings;
            this.builder = builder;
            tenantStores = new Dictionary<string, Elders.Cronus.EventStore.IEventStore>();
            tenantPlayers = new Dictionary<string, Elders.Cronus.EventStore.IEventStorePlayer>();
        }

        public Elders.Cronus.EventStore.IEventStore GetEventStore(string tenant)
        {
            if (tenantStores.ContainsKey(tenant) == false)
                InitializeTenant(tenant);

            return tenantStores[tenant];
        }

        public Elders.Cronus.EventStore.IEventStorePlayer GetEventStorePlayer(string tenant)
        {
            if (tenantStores.ContainsKey(tenant) == false)
                InitializeTenant(tenant);

            return tenantPlayers[tenant];
        }

        void InitializeTenant(string tenant)
        {
            var keyspace = $"{tenant}_{settings.Keyspace}";
            if (keyspace.Length > 48) throw new System.ArgumentException($"Cassandra keyspace exceeds maximum length of 48. Keyspace: {keyspace}");

            DataStaxCassandra.Cluster cluster = null;
            if (ReferenceEquals(null, settings.Cluster))
            {
                cluster = DataStaxCassandra.Cluster
                    .Builder()
                    .WithReconnectionPolicy(settings.ReconnectionPolicy)
                    .WithRetryPolicy(settings.RetryPolicy)
                    .WithConnectionString(settings.ConnectionString)
                    .Build();
            }
            else
            {
                cluster = settings.Cluster;
            }

            var session = cluster.Connect();
            var storageManager = new Elders.Cronus.Persistence.Cassandra.CassandraEventStoreStorageManager(session, keyspace, settings.EventStoreTableNameStrategy, settings.ReplicationStrategy);
            storageManager.CreateStorage();
            session.ChangeKeyspace(keyspace);
            var serializer = builder.Container.Resolve<Elders.Cronus.Serializer.ISerializer>();
            string bc = (this.settings as Elders.Cronus.EventStore.Config.IEventStoreSettings).BoundedContext;
            var eventStore = new Elders.Cronus.Persistence.Cassandra.CassandraEventStore(session, settings.EventStoreTableNameStrategy, serializer, settings.WriteConsistencyLevel, settings.ReadConsistencyLevel);
            var player = new Elders.Cronus.Persistence.Cassandra.CassandraEventStorePlayer(session, settings.EventStoreTableNameStrategy, bc, serializer);

            tenantStores.Add(tenant, eventStore);
            tenantPlayers.Add(tenant, player);
        }
    }
}
