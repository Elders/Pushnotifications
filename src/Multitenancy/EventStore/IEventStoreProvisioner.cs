using Elders.Cronus.EventStore;

namespace Multitenancy.EventStore
{
    public interface IEventStoreProvisioner
    {
        IEventStorePlayer GetEventStorePlayer(string tenant);
        IEventStore GetEventStore(string tenant);
    }
}
