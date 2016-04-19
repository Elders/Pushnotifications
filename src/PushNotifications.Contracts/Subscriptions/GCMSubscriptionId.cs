using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "365063be-54fa-43b8-9241-fe3c6002b9ba")]
    public class GCMSubscriptionId : StringId
    {
        protected GCMSubscriptionId() { }
        public GCMSubscriptionId(string id) : base(id, "Subscription") { }
    }
}