using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "4b17a6bc-5b7a-4396-9d1c-be8c5dc37912")]
    public sealed class PushNotificationId : AggregateRootId
    {
        PushNotificationId() { }

        public PushNotificationId(string tenant, string id) : base(tenant, "pushnotification", id) { }
    }
}
