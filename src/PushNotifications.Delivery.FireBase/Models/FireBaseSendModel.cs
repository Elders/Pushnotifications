using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PushNotifications.Contracts;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseSendModel
    {
        const long MAX_TTL_SECONDS = 1209600; // 4 weeks

        public FireBaseSendModel(IList<string> tokens, FireBaseSendNotificationModel notificationPayload, Dictionary<string, object> notificationData, Timestamp expiresAt, bool contentAvailable)
        {
            if (ReferenceEquals(null, tokens) || tokens.Count < 0 || tokens.Count > 1000) throw new ArgumentException(nameof(tokens));
            if (ReferenceEquals(null, notificationPayload)) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, expiresAt)) throw new ArgumentNullException(nameof(expiresAt));
            if (ReferenceEquals(null, notificationData)) throw new ArgumentNullException(nameof(notificationData));

            RegistrationIds = new List<string>(tokens);
            Notification = notificationPayload;
            TTL = ExpirationTimeToSeconds(expiresAt);
            ContentAvailable = contentAvailable;
            Data = notificationData;
        }

        [JsonProperty(PropertyName = "Registration_ids")]
        public List<string> RegistrationIds { get; private set; }

        [JsonProperty(PropertyName = "Content_available")]
        public bool ContentAvailable { get; private set; }

        public FireBaseSendNotificationModel Notification { get; private set; }

        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// This parameter specifies how long (in seconds) the message should be kept in FCM storage if the device is offline.
        /// The maximum time to live supported is 4 weeks, and the default value is 4 weeks.
        /// </summary>
        [JsonProperty(PropertyName = "Time_to_live")]
        public long TTL { get; private set; }

        long ExpirationTimeToSeconds(Timestamp t)
        {
            var utcNow = DateTime.UtcNow;
            var difference = t.DateTime - utcNow;

            if (difference.TotalSeconds > MAX_TTL_SECONDS)
                return MAX_TTL_SECONDS;

            return (long)difference.TotalSeconds;
        }
    }
}
