using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;

namespace Multitenancy.TenantResolver
{
    public interface ITenantResolver
    {
        string Resolve(IAggregateRootId id);

        string Resolve(AggregateCommit aggregateCommit);
    }
}
