using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications.Events
{
    [DataContract(Name = "ecfd29f3-ed2f-40f7-8ade-d2ae80355fb8")]
    public class PushNotificationWasSent : IEvent
    {
        PushNotificationWasSent() { }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; private set; }
    }
}
