using DataStaxCassandra = Cassandra;
using Elders.Cronus.Projections.Cassandra.ReplicationStrategies;

namespace Multitenancy.Cassandra.Projections
{
    public static class CasssandraExtensions
    {
        public static void CreateKeyspace(this DataStaxCassandra.ISession session, ICassandraReplicationStrategy replicationStrategy, string keyspace)
        {
            var createKeySpaceQuery = replicationStrategy.CreateKeySpaceTemplate(keyspace);
            session.Execute(createKeySpaceQuery);
            session.ChangeKeyspace(keyspace);
        }
    }
}
