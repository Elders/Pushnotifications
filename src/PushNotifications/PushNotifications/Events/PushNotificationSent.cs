using Elders.Cronus;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using PushNotifications.Subscriptions;

namespace PushNotifications.Contracts.PushNotifications.Events
{
    [DataContract(Name = "d4cad1bf-838a-4839-a8b4-d4052a45d132")]
    public class PushNotificationSent : IEvent
    {
        PushNotificationSent() { }

        public PushNotificationSent(PushNotificationId id, DeviceSubscriberId subscriberId, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable)
        {
            if (id is null == true) throw new ArgumentNullException(nameof(id));
            if (subscriberId is null == true) throw new ArgumentNullException(nameof(subscriberId));
            if (notificationPayload is null == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (notificationData is null == true) throw new ArgumentNullException(nameof(notificationData));

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
        public DeviceSubscriberId SubscriberId { get; private set; }

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
