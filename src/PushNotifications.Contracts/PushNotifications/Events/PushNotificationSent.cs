using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications.Events
{
    [DataContract(Name = "4d9cb0ed-9167-4f8e-9b31-9fd5e94e45e3")]
    public class PushNotificationSent : IEvent
    {
        PushNotificationSent() { }

        public PushNotificationSent(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload)
        {
            Id = id;
            SubscriberId = subscriberId;
            NotificationPayload = notificationPayload;
        }

        [DataMember(Order = 1)]
        public PushNotificationId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public NotificationPayload NotificationPayload { get; private set; }

        // We may want to support DataPayload at some point

        public override string ToString()
        {
            return $"Push notification with Id {Id.Urn.Value} was sent to {SubscriberId.Urn.Value} with NotificationPayload: {NotificationPayload}";
        }
    }
}
