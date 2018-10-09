namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTrackerFactory
    {
        ITopicSubscriptionTracker GetService(string tenant);
    }
}
