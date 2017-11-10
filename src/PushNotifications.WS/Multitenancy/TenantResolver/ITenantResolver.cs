using Elders.Cronus.DomainModeling;
using Elders.Cronus.EventStore;

namespace PushNotifications.WS.Multitenancy
{
    public interface ITenantResolver
    {
        string Resolve(IAggregateRootId id);

        string Resolve(AggregateCommit aggregateCommit);
    }
}
