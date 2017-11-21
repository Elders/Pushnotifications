using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.PushNotifications.Commands
{
    [DataContract(Name = "5e1e1ae0-d1d6-4243-92fc-b0b6652ecb5b")]
    public class SendPushNotification : ICommand
    {
        public SendPushNotification(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            Id = id;
            SubscriberId = subscriberId;
            NotificationPayload = notificationPayload;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        [DataMember(Order = 1)]
        public PushNotificationId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public NotificationPayload NotificationPayload { get; private set; }

        [DataMember(Order = 4)]
        public Timestamp ExpiresAt { get; private set; }

        /// <summary>
        /// On iOS, use this field to represent content-available in the APNs payload.
        /// When a notification or message is sent and this is set to true,
        /// an inactive client app is awoken, and the message is sent through APNs as a silent notification and not through the FCM connection server.
        /// Note that silent notifications in APNs are not guaranteed to be delivered, and can depend on factors such as the user turning on Low Power Mode,
        /// force quitting the app, etc. On Android, data messages wake the app by default.
        /// On Chrome, currently not supported.
        /// </summary>
        [DataMember(Order = 5)]
        public bool ContentAvailable { get; private set; }

        public override string ToString()
        {
            return $"Send Push notification with Id '{Id.Urn.Value}' to '{SubscriberId.Urn.Value}'. NotificationPayload: {NotificationPayload}";
        }
    }
}
