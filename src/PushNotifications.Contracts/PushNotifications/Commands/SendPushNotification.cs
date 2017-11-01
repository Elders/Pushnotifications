using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications.Commands
{
    [DataContract(Name = "5e1e1ae0-d1d6-4243-92fc-b0b6652ecb5b")]
    public class SendPushNotification : ICommand
    {
        public SendPushNotification(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload)
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

        public bool IsValid()
        {
            return
               StringTenantId.IsValid(Id) && StringTenantId.IsValid(SubscriberId) && ReferenceEquals(null, NotificationPayload) == false;
        }

        public override string ToString()
        {
            return $"Send Push notification with Id '{Id.Urn.Value}' to '{SubscriberId.Urn.Value}'. NotificationPayload: {NotificationPayload}";
        }
    }
}
