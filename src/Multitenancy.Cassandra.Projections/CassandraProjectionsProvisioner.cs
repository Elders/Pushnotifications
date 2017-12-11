using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using Elders.Cronus.IocContainer;
using DataStaxCassandra = Cassandra;
using System.Collections.Generic;
using System;

namespace Multitenancy.Cassandra.Projections
{
    public class CassandraProjectionsProvisioner : ICassandraProjectionsProvisioner
    {
        readonly ISettingsBuilder builder;

        readonly ICassandraProjectionsStoreSettings settings;

        readonly Dictionary<string, IProjectionStore> tenantProjectionStores;

        readonly Dictionary<string, ISnapshotStore> tenantSnapshotStores;

        public CassandraProjectionsProvisioner(ISettingsBuilder builder, ICassandraProjectionsStoreSettings settings)
        {
            if (ReferenceEquals(null, builder) == true) throw new ArgumentNullException(nameof(builder));
            if (ReferenceEquals(null, settings) == true) throw new ArgumentNullException(nameof(settings));

            this.builder = builder;
            this.settings = settings;

            tenantProjectionStores = new Dictionary<string, IProjectionStore>();
            tenantSnapshotStores = new Dictionary<string, ISnapshotStore>();
        }

        void InitializeTenant(string tenant)
        {
            if (string.IsNullOrEmpty(tenant) == true) throw new ArgumentNullException(nameof(tenant));

            var keyspace = $"{tenant}_{settings.Keyspace}";
            if (keyspace.Length > 48) throw new ArgumentException($"Cassandra keyspace exceeds maximum length of 48. Keyspace: {keyspace}");

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
            session.CreateKeyspace(settings.ReplicationStrategy, keyspace);
            var serializer = builder.Container.Resolve<Elders.Cronus.Serializer.ISerializer>();

            var versionStore = new CassandraVersionStore(session);
            var projectionStore = new CassandraProjectionStore(settings.ProjectionTypes, session, serializer, versionStore);
            var snapshotStore = new CassandraSnapshotStore(settings.ProjectionsToSnapshot, session, serializer, versionStore);

            tenantProjectionStores.Add(tenant, projectionStore);
            tenantSnapshotStores.Add(tenant, snapshotStore);
        }

        public IProjectionStore GetProjectionStore(string tenant)
        {
            if (string.IsNullOrEmpty(tenant) == true) throw new ArgumentNullException(nameof(tenant));

            if (tenantProjectionStores.ContainsKey(tenant) == false)
                InitializeTenant(tenant);

            return tenantProjectionStores[tenant];
        }

        public ISnapshotStore GetSnapshotStore(string tenant)
        {
            if (string.IsNullOrEmpty(tenant) == true) throw new ArgumentNullException(nameof(tenant));

            if (tenantSnapshotStores.ContainsKey(tenant) == false)
                InitializeTenant(tenant);

            return tenantSnapshotStores[tenant];
        }
    }
}
