using System;
using DataStaxCassandra = Cassandra;
using Elders.Cronus.Persistence.Cassandra.Config;

namespace Multitenancy.Cassandra
{
    public static class MultiTenantCassandraEventStore
    {
        public static T UseMultiTenantCassandraEventStore<T>(this T self, Action<MultiTenantCassandraEventStoreSettings> configure) where T : Elders.Cronus.Pipeline.Config.IConsumerSettings<Elders.Cronus.DomainModeling.ICommand>
        {
            MultiTenantCassandraEventStoreSettings settings = new MultiTenantCassandraEventStoreSettings(self);
            settings.SetReconnectionPolicy(new DataStaxCassandra.ExponentialReconnectionPolicy(100, 100000));
            settings.SetRetryPolicy(new DataStaxCassandra.DefaultRetryPolicy());
            settings.SetReplicationStrategy(new Elders.Cronus.Persistence.Cassandra.ReplicationStrategies.SimpleReplicationStrategy(1));
            settings.SetWriteConsistencyLevel(DataStaxCassandra.ConsistencyLevel.All);
            settings.SetReadConsistencyLevel(DataStaxCassandra.ConsistencyLevel.Quorum);
            configure?.Invoke(settings);

            (settings as Elders.Cronus.Pipeline.Config.ISettingsBuilder).Build();
            return self;
        }

    }
}
