using Elders.Cronus;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using PushNotifications.Contracts.PushNotifications;

namespace PushNotifications.PushNotifications.Commands;

[DataContract(Name = "61f3b13a-0973-4bf9-8c29-d42381629eef")]
public sealed class TopicSendPushNotification : ICommand
{
    TopicSendPushNotification()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }

    public TopicSendPushNotification(TopicPushNotificationId id, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable) : this()
    {
        if (id.IsValid() == false) throw new ArgumentException(nameof(id));
        if (notificationPayload is null == true) throw new ArgumentNullException(nameof(notificationPayload));
        if (notificationData is null == true) throw new ArgumentNullException(nameof(notificationData));

        Id = id;
        NotificationPayload = notificationPayload;
        NotificationData = notificationData;
        ExpiresAt = expiresAt;
        ContentAvailable = contentAvailable;
    }

    [DataMember(Order = 1)]
    public TopicPushNotificationId Id { get; private set; }

    [DataMember(Order = 2)]
    public NotificationPayload NotificationPayload { get; private set; }

    [DataMember(Order = 3)]
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// On iOS, use this field to represent content-available in the APNs payload.
    /// When a notification or message is sent and this is set to true,
    /// an inactive client app is awoken, and the message is sent through APNs as a silent notification and not through the FCM connection server.
    /// Note that silent notifications in APNs are not guaranteed to be delivered, and can depend on factors such as the user turning on Low Power Mode,
    /// force quitting the app, etc. On Android, data messages wake the app by default.
    /// On Chrome, currently not supported.
    /// </summary>
    [DataMember(Order = 4)]
    public bool ContentAvailable { get; private set; }

    [DataMember(Order = 5)]
    public Dictionary<string, object> NotificationData { get; private set; }

    [DataMember(Order = 6)]
    public DateTimeOffset Timestamp { get; private set; }
}
