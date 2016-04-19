using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "6acec9cb-1d22-45f5-bce8-e5f69230fa8e")]
    public class ParseSubscriptionId : StringId
    {
        protected ParseSubscriptionId() { }
        public ParseSubscriptionId(string id) : base(id, "Subscription") { }
    }
}