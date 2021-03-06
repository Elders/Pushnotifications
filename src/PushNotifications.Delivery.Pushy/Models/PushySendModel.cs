﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PushNotifications.Contracts;

namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushySendModel
    {
        const long MAX_TTL_SECONDS = 31536000; // 1 year

        public PushySendModel(IList<string> tokens, PushySendNotificationModel notification, Dictionary<string, object> data, Timestamp expiresAt, bool contentAvailable)
        {
            if (ReferenceEquals(null, tokens) == true || tokens.Count == 0) throw new ArgumentException(nameof(tokens));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (ReferenceEquals(null, data) == true) throw new ArgumentNullException(nameof(data));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            RegistrationIds = new List<string>(tokens);
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

        public PushySendModel(Topic topic, PushySendNotificationModel notification, Dictionary<string, object> data, Timestamp expiresAt, bool contentAvailable)
        {
            if (ReferenceEquals(null, topic)) throw new ArgumentException(nameof(topic));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (ReferenceEquals(null, data) == true) throw new ArgumentNullException(nameof(data));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

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

        [JsonProperty(PropertyName = "Registration_ids")]
        public List<string> RegistrationIds { get; private set; }

        [JsonProperty(PropertyName = "to")]
        public string Topic { get; private set; }

        [JsonProperty(PropertyName = "Content_available")]
        public bool ContentAvailable { get; private set; }

        public PushySendNotificationModel Notification { get; private set; }

        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Specifies how long (in seconds) the push notification should be kept if the device is offline.
        /// The default value is 1 month.The maximum value is 1 year.
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
