using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Contracts;

namespace PushNotifications.Projections
{
    [DataContract(Name = "4cc819b1-6f29-4cdd-a4d8-0eeb37cb88ec")]
    public class SubscriberTokens
    {
        public SubscriberTokens()
        {
            TokenTypePairs = new HashSet<SubscriptionTokenSubscriptionTypePair>();
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionTokenSubscriptionTypePair> TokenTypePairs { get; set; }
    }
}
