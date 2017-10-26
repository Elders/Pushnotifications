using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.FireBaseSubscriptions
{
    [DataContract(Name = "0d87e0ad-b889-465c-988f-fea80eaa2d51")]
    public class FireBaseSubscriptionId : StringTenantId
    {
        FireBaseSubscriptionId() { }

        public FireBaseSubscriptionId(IUrn urn) : base(urn, "firebasesubscription") { }


        public FireBaseSubscriptionId(string id, string tenant) : base(id, "firebasesubscription", tenant) { }
    }
}
