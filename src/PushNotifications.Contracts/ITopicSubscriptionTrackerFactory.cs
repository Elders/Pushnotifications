namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTrackerFactory
    {
        ITopicSubscriptionTracker GetService(string tenant);
    }

    public interface IBadgeCountTrackerFactory
    {
        IBadgeCountTracker GetService(string tenant);
    }
}
