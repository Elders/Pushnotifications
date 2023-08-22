using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseSendModel
    {
        const long MAX_TTL_SECONDS = 432000; // 5 days

        public FireBaseSendModel(List<string> tokens, FireBaseSendNotificationModel notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt)
        {
            if (ReferenceEquals(null, tokens) || tokens.Count == 0 || tokens.Count > 1000) throw new ArgumentException(nameof(tokens));
            if (ReferenceEquals(null, notificationPayload)) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, expiresAt)) throw new ArgumentNullException(nameof(expiresAt));
            if (ReferenceEquals(null, notificationData)) throw new ArgumentNullException(nameof(notificationData));

            RegistrationIds = tokens;
            Notification = notificationPayload;
            TTL = ExpirationTimeToSeconds(expiresAt);
            Data = notificationData;
            Priority = "high";
            MutableContent = true;
        }

        /// <summary>
        /// This parameter specifies the recipient of a multicast message, a message sent to more than one registration token.
        /// The value should be an array of registration tokens to which to send the multicast message. The array must contain at least 1 and at most 1000 registration tokens.
        /// To send a message to a single device, use the to parameter.
        /// </summary>
        [JsonPropertyName("registration_ids")]
        public List<string> RegistrationIds { get; private set; }

        /// <summary>
        /// This parameter specifies the predefined, user-visible key-value pairs of the notification payload. See Notification payload support for detail.
        /// For more information about notification message and data message options, see Message types. If a notification payload is provided, or the content_available
        /// option is set to true for a message to an iOS device, the message is sent through APNs, otherwise it is sent through the FCM connection server.
        /// </summary>
        [JsonPropertyName("notification")]
        public FireBaseSendNotificationModel Notification { get; private set; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Sets the priority of the message. Valid values are "normal" and "high." On iOS, these correspond to APNs priorities 5 and 10.
        /// By default, notification messages are sent with high priority, and data messages are sent with normal priority. Normal priority optimizes the client app's
        /// battery consumption and should be used unless immediate delivery is required. For messages with normal priority, the app may receive the message with unspecified delay.
        /// When a message is sent with high priority, it is sent immediately, and the app can display a notification.
        /// </summary>
        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        /// <summary>
        /// This parameter specifies how long (in seconds) the message should be kept in FCM storage if the device is offline.
        /// The maximum time to live supported is 4 weeks, and the default value is 4 weeks.
        /// </summary>
        [JsonPropertyName("time_to_live")]
        public long TTL { get; private set; }


        /// <summary>
        /// Currently for iOS 10+ devices only. On iOS, use this field to represent mutable-content in the APNs payload.
        /// When a notification is sent and this is set to true, the content of the notification can be modified before it is displayed, using a Notification Service app extension.
        /// This parameter will be ignored for Android and web.
        /// </summary>
        [JsonPropertyName("mutable_content")]
        public bool MutableContent { get; private set; }

        long ExpirationTimeToSeconds(DateTimeOffset t)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var difference = t - utcNow;

            if (difference.TotalSeconds > MAX_TTL_SECONDS)
                return MAX_TTL_SECONDS;

            return (long)difference.TotalSeconds;
        }
    }
}
