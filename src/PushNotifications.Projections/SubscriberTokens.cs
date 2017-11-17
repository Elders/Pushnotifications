using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Contracts;

namespace PushNotifications.Projections
{
    [DataContract(Name = "f648e05a-b5d9-4947-ade6-789b7ffb3601")]
    public class SubscriberTokens
    {
        public SubscriberTokens()
        {
            Tokens = new HashSet<SubscriptionToken>();
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionToken> Tokens { get; set; }
    }
}
