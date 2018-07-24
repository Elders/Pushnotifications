using System.Collections.Generic;

namespace PushNotifications.Contracts
{
    public interface ITopicSubscriptionTracker
    {
        void Decrement(string name);
        void Increment(string name);
        IEnumerable<StatCounter> Show(string name);
    }

    public class StatCounter : Elders.Cronus.ValueObject<StatCounter>
    {
        public StatCounter(string name, long count)
        {
            if (string.IsNullOrEmpty(name)) throw new System.ArgumentNullException(nameof(name));

            Name = name;
            Count = count;
        }

        public string Name { get; private set; }

        public long Count { get; private set; }
    }
}
