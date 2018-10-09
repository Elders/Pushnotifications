namespace PushNotifications.Contracts
{
    public interface IBadgeCountTrackerFactory
    {
        IBadgeCountTracker GetService(string tenant);
    }
}
