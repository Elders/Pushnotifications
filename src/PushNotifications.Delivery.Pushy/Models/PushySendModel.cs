using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushySendModel
    {
        const long MAX_TTL_SECONDS = 31536000; // 1 year

        public PushySendModel(IList<string> tokens, PushySendNotificationModel notification, Dictionary<string, object> data, DateTimeOffset expiresAt, bool contentAvailable)
        {
            if (tokens is null == true || tokens.Count == 0) throw new ArgumentException(nameof(tokens));
            if (notification is null == true) throw new ArgumentNullException(nameof(notification));
            if (data is null == true) throw new ArgumentNullException(nameof(data));

            Tokens = new List<string>(tokens);
            Notification = notification;
            Data = new Dictionary<string, object>();
            TTL = ExpirationTimeToSeconds(expiresAt);
            ContentAvailable = contentAvailable;


            Data.Add("title", notification.Title);
            Data.Add("body", notification.Body);
            Data.Add("sound", notification.Sound);
            Data.Add("badge", notification.Badge);

            foreach (var item in data)
            {
                if (Data.ContainsKey(item.Key) == true)
                    Data[item.Key] = item.Value;
                else
                    Data.Add(item.Key, item.Value);
            }
        }

        [JsonPropertyName("to")]
        public List<string> Tokens { get; private set; }

        [JsonPropertyName("Content_available")]
        public bool ContentAvailable { get; private set; }

        [JsonPropertyName("notification")]
        public PushySendNotificationModel Notification { get; private set; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Specifies how long (in seconds) the push notification should be kept if the device is offline.
        /// The default value is 1 month.The maximum value is 1 year.
        /// </summary>
        [JsonPropertyName("Time_to_live")]
        public long TTL { get; private set; }

        long ExpirationTimeToSeconds(DateTimeOffset t)
        {
            var utcNow = DateTime.UtcNow;
            var difference = t.UtcDateTime - utcNow;

            if (difference.TotalSeconds > MAX_TTL_SECONDS)
                return MAX_TTL_SECONDS;

            return (long)difference.TotalSeconds;
        }
    }

    public class PushySendToTopicModel
    {
        const long MAX_TTL_SECONDS = 31536000; // 1 year

        public PushySendToTopicModel(Topic topic, PushySendNotificationModel notification, Dictionary<string, object> data, DateTimeOffset expiresAt, bool contentAvailable)
        {
            if (topic is null) throw new ArgumentException(nameof(topic));
            if (notification is null == true) throw new ArgumentNullException(nameof(notification));
            if (data is null == true) throw new ArgumentNullException(nameof(data));

            Topic = "/topics/" + topic;
            Notification = notification;
            Data = new Dictionary<string, object>();
            TTL = ExpirationTimeToSeconds(expiresAt);
            ContentAvailable = contentAvailable;


            Data.Add("title", notification.Title);
            Data.Add("body", notification.Body);
            Data.Add("sound", notification.Sound);
            Data.Add("badge", notification.Badge);

            foreach (var item in data)
            {
                if (Data.ContainsKey(item.Key) == true)
                    Data[item.Key] = item.Value;
                else
                    Data.Add(item.Key, item.Value);
            }
        }

        [JsonPropertyName("to")]
        public string Topic { get; private set; }

        [JsonPropertyName("Content_available")]
        public bool ContentAvailable { get; private set; }

        public PushySendNotificationModel Notification { get; private set; }

        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Specifies how long (in seconds) the push notification should be kept if the device is offline.
        /// The default value is 1 month.The maximum value is 1 year.
        /// </summary>
        [JsonPropertyName("Time_to_live")]
        public long TTL { get; private set; }

        long ExpirationTimeToSeconds(DateTimeOffset t)
        {
            var utcNow = DateTime.UtcNow;
            var difference = t.UtcDateTime - utcNow;

            if (difference.TotalSeconds > MAX_TTL_SECONDS)
                return MAX_TTL_SECONDS;

            return (long)difference.TotalSeconds;
        }
    }
}
