using System;
using System.Linq;
using Elders.Cronus.Pipeline.Config;
using Elders.Cronus.Projections.Cassandra.Config;
using Elders.Cronus.Projections.Cassandra.ReplicationStrategies;
using Elders.Cronus.Projections.Cassandra.Snapshots;
using DataStaxCassandra = Cassandra;

namespace Multitenancy.Cassandra.Projections
{
    public static class MultiTenantCassandraProjections
    {
        public static T UseMultiTenantCassandraProjections<T>(this T self, Action<MultiTenantCassandraProjectionsSettings> configure) where T : ISubscrptionMiddlewareSettings
        {
            if (ReferenceEquals(null, configure) == true) throw new ArgumentNullException(nameof(configure));

            MultiTenantCassandraProjectionsSettings settings = new MultiTenantCassandraProjectionsSettings(self, self as ISubscrptionMiddlewareSettings);
            settings.SetProjectionsReconnectionPolicy(new DataStaxCassandra.ExponentialReconnectionPolicy(100, 100000));
            settings.SetProjectionsRetryPolicy(new NoHintedHandOffRetryPolicy());
            settings.SetProjectionsReplicationStrategy(new SimpleReplicationStrategy(1));
            settings.SetProjectionsWriteConsistencyLevel(DataStaxCassandra.ConsistencyLevel.All);
            settings.SetProjectionsReadConsistencyLevel(DataStaxCassandra.ConsistencyLevel.Quorum);
            settings.UseSnapshotStrategy(new DefaultSnapshotStrategy(snapshotOffset: TimeSpan.FromDays(1), eventsInSnapshot: 500));

            (settings as ICassandraProjectionsStoreSettings).ProjectionTypes = self.HandlerRegistrations;

            configure?.Invoke(settings);

            (settings as ISettingsBuilder).Build();
            return self;
        }

        public static T ConfigureMultiTenantCassandraProjectionsStore<T>(this T self, Action<MultiTenantCassandraProjectionsStoreSettings> configure) where T : ISettingsBuilder
        {
            if (ReferenceEquals(null, configure) == true) throw new ArgumentNullException(nameof(configure));

            MultiTenantCassandraProjectionsStoreSettings settings = new MultiTenantCassandraProjectionsStoreSettings(self);
            settings.SetProjectionsReconnectionPolicy(new DataStaxCassandra.ExponentialReconnectionPolicy(100, 100000));
            settings.SetProjectionsRetryPolicy(new NoHintedHandOffRetryPolicy());
            settings.SetProjectionsReplicationStrategy(new SimpleReplicationStrategy(1));
            settings.SetProjectionsWriteConsistencyLevel(DataStaxCassandra.ConsistencyLevel.All);
            settings.SetProjectionsReadConsistencyLevel(DataStaxCassandra.ConsistencyLevel.Quorum);
            settings.UseSnapshotStrategy(new DefaultSnapshotStrategy(snapshotOffset: TimeSpan.FromDays(1), eventsInSnapshot: 500));

            configure?.Invoke(settings);

            var projectionTypes = (settings as ICassandraProjectionsStoreSettings).ProjectionTypes;

            if (ReferenceEquals(null, projectionTypes) || projectionTypes.Any() == false)
                throw new InvalidOperationException("No projection types are registerd. Please use SetProjectionTypes.");

            (settings as ISettingsBuilder).Build();
            return self;
        }
    }
}
