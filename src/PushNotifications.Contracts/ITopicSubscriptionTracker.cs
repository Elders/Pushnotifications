using System.Collections.Generic;

namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTracker
    {
        void Decrement(string name);
        void Increment(string name);
        IEnumerable<StatCounter> Show(string name);
    }
}
