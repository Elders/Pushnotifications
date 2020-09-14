using Elders.Cronus;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.PushNotifications.Commands
{
    [DataContract(Name = "5e1e1ae0-d1d6-4243-92fc-b0b6652ecb5b")]
    public class SendPushNotification : ICommand
    {
        public SendPushNotification(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable)
        {
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));
            if (ReferenceEquals(null, subscriberId) == true) throw new ArgumentNullException(nameof(subscriberId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, notificationData) == true) throw new ArgumentNullException(nameof(notificationData));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            Id = id;
            SubscriberId = subscriberId;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
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
        public DateTimeOffset ExpiresAt { get; private set; }

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

        [DataMember(Order = 6)]
        public Dictionary<string, object> NotificationData { get; private set; }
    }
}
