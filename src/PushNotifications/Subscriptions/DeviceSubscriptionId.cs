using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "163c8f59-e89b-499e-8d69-6e11af98c817")]
    public sealed class DeviceSubscriptionId : AggregateRootId
    {
        DeviceSubscriptionId() { }

        public DeviceSubscriptionId(string tenant, string id) : base(tenant, "subscription", id) { }
    }
}
