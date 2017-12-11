using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "4b17a6bc-5b7a-4396-9d1c-be8c5dc37912")]
    public class PushNotificationId : StringTenantId
    {
        PushNotificationId() { }

        public PushNotificationId(IUrn urn) : base(urn, "pushnotification") { }

        public PushNotificationId(string id, string tenant) : base(id, "pushnotification", tenant) { }
    }
}
