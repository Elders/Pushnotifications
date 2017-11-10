using Elders.Cronus.EventStore;

namespace PushNotifications.WS.Multitenancy
{
    public interface IEventStoreProvisioner
    {
        IEventStorePlayer GetEventStorePlayer(string tenant);
        IEventStore GetEventStore(string tenant);
    }
}
