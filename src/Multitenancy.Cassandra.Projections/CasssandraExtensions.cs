using DataStaxCassandra = Cassandra;
using Elders.Cronus.Projections.Cassandra.ReplicationStrategies;
using System;

namespace Multitenancy.Cassandra.Projections
{
    public static class CasssandraExtensions
    {
        public static void CreateKeyspace(this DataStaxCassandra.ISession session, ICassandraReplicationStrategy replicationStrategy, string keyspace)
        {
            if (ReferenceEquals(null, replicationStrategy) == true) throw new ArgumentNullException(nameof(replicationStrategy));
            if (string.IsNullOrEmpty(keyspace) == true) throw new ArgumentNullException(nameof(keyspace));

            var createKeySpaceQuery = replicationStrategy.CreateKeySpaceTemplate(keyspace);
            session.Execute(createKeySpaceQuery);
            session.ChangeKeyspace(keyspace);
        }
    }
}
