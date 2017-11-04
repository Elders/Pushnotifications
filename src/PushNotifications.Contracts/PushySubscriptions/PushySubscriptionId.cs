using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushySubscriptions
{
    [DataContract(Name = "3227a15d-16b9-4d43-a902-94ace57d415a")]
    public class PushySubscriptionId : StringTenantId
    {
        PushySubscriptionId() { }

        public PushySubscriptionId(IUrn urn) : base(urn, "pushysubscription") { }


        public PushySubscriptionId(string id, string tenant) : base(id, "pushysubscription", tenant) { }
    }
}
