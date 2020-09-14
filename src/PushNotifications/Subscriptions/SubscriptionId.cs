using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "163c8f59-e89b-499e-8d69-6e11af98c817")]
    public sealed class SubscriptionId : AggregateRootId<SubscriptionId>
    {
        SubscriptionId() { }

        SubscriptionId(string id, string tenant) : base(id, "subscription", tenant) { }

        protected override SubscriptionId Construct(string id, string tenant)
        {
            return new SubscriptionId(id, tenant);
        }
    }
}
