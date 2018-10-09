namespace PushNotifications.Contracts
{
    public interface IBadgeCountTracker
    {
        void SetCount(string subscriberId, int badgeCount);
        void Increment(string subscriberId);
        StatCounter Show(string name);
    }
}
