using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Elders.Cronus.Projections.Cassandra.Snapshots;

namespace Multitenancy.Cassandra.Projections
{
    public interface ICassandraProjectionsProvisioner
    {
        IProjectionStore GetProjectionStore(string tenant);
        ISnapshotStore GetSnapshotStore(string tenant);
    }
}
