namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTracker
    {
        void Decrement(string name);
        void Increment(string name);
        StatCounter Show(string name);
    }
}
