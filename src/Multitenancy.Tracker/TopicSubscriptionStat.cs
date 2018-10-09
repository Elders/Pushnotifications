using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace Multitenancy.Tracker
{
    public partial class TopicSubscriptionTracker
    {
        [DataContract(Name = "ece0a261-027e-479b-9ce3-530d1acd8dfe")]
        public class TopicSubscriptionStat : ValueObject<TopicSubscriptionStat>
        {
            public TopicSubscriptionStat(string name)
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

                Name = name;
            }

            [DataMember(Order = 1)]
            public string Name { get; private set; }
        }
    }

    [DataContract(Name = "25e78259-9a07-4c5f-9d48-fbd4389bd21e")]
    public class BadgeStatTracker : ValueObject<BadgeStatTracker>
    {
        public BadgeStatTracker(string subscriberId)
        {
            if (string.IsNullOrEmpty(subscriberId)) throw new ArgumentNullException(nameof(subscriberId));

            SubscriberId = subscriberId;
        }

        [DataMember(Order = 1)]
        public string SubscriberId { get; private set; }
    }
}
