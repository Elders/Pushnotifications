using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "cc411286-f3e0-43b8-89e9-0631b8203c1a")]
    public class APNSSubscriptionId : StringId
    {
        protected APNSSubscriptionId() { }
        public APNSSubscriptionId(string id) : base(id, "Subscription") { }
    }
}