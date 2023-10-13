using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PushNotifications.Subscriptions;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseSubscriptionModel
    {
        public FireBaseSubscriptionModel(List<string> registrationTokens, Topic topic)
        {
            if (registrationTokens is null) throw new ArgumentNullException(nameof(registrationTokens));
            if (topic is null) throw new ArgumentNullException(nameof(topic));

            RegistrationTokens = registrationTokens;
            Topic = "/topics/" + topic;
        }

        public FireBaseSubscriptionModel(string registrationToken, Topic topic)
        {
            if (string.IsNullOrEmpty(registrationToken)) throw new ArgumentNullException(nameof(registrationToken));
            if (topic is null) throw new ArgumentNullException(nameof(topic));

            RegistrationTokens = new List<string>() { registrationToken };
            Topic = "/topics/" + topic;
        }

        [JsonPropertyName("registration_tokens")]
        public List<string> RegistrationTokens { get; private set; }

        [JsonPropertyName("to")]
        public string Topic { get; private set; }
    }
}
