using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "163c8f59-e89b-499e-8d69-6e11af98c817")]
    public class SubscriptionId : StringTenantId
    {
        SubscriptionId() { }

        public SubscriptionId(IUrn urn) : base(urn, "subscription") { }


        public SubscriptionId(string id, string tenant) : base(id, "subscription", tenant) { }
    }
}
