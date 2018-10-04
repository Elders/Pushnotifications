using System.Collections.Generic;

namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTracker
    {
        void Decrement(string name);
        void Increment(string name);
        StatCounter Show(string name);
    }

    public interface IBadgeCountTracker
    {
        void SetCount(string subscriberId, int badgeCount);
        void Increment(string subscriberId);
    }
}
