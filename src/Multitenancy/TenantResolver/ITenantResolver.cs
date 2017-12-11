using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;
using Elders.Cronus.Projections;

namespace Multitenancy.TenantResolver
{
    public interface ITenantResolver
    {
        string Resolve(IAggregateRootId id);

        string Resolve(AggregateCommit aggregateCommit);

        string Resolve(ProjectionCommit projectionCommit);

        string Resolve(IBlobId id);
    }
}
