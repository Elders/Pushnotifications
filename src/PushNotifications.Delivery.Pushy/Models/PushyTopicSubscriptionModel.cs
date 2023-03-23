using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PushNotifications.Subscriptions;

namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushyTopicSubscriptionModel
    {
        public PushyTopicSubscriptionModel(string token, Topic topic)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException(nameof(token));
            if (ReferenceEquals(null, topic)) throw new ArgumentException(nameof(topic));

            Token = token;
            Topics = new List<string>() { topic };
        }

        public PushyTopicSubscriptionModel(string token, IEnumerable<Topic> topics)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException(nameof(token));
            if (ReferenceEquals(null, topics)) throw new ArgumentException(nameof(topics));

            Token = token;
            Topics = new List<string>();
            foreach (Topic topic in topics)
            {
                Topics.Add(topic);
            }
        }

        [JsonPropertyName("token")]
        public string Token { get; private set; }

        [JsonPropertyName("topics")]
        public List<string> Topics { get; private set; }
    }
}
