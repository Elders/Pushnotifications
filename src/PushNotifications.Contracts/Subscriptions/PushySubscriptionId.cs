using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "83c585ea-39d0-490e-96ea-2319e9adc060")]
    public class PushySubscriptionId : StringId
    {
        protected PushySubscriptionId() { }
        public PushySubscriptionId(string id) : base(id, "Subscription") { }
    }
}
