using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PushNotifications.Contracts;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseSubscriptionModel
    {
        public FireBaseSubscriptionModel(List<string> registrationTokens, Topic topic)
        {
            if (ReferenceEquals(null, registrationTokens)) throw new ArgumentNullException(nameof(registrationTokens));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            RegistrationTokens = registrationTokens;
            Topic = "/topics/" + topic;
        }

        public FireBaseSubscriptionModel(string registrationToken, Topic topic)
        {
            if (string.IsNullOrEmpty(registrationToken)) throw new ArgumentNullException(nameof(registrationToken));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            RegistrationTokens = new List<string>() { registrationToken };
            Topic = "/topics/" + topic;
        }

        [JsonProperty(PropertyName = "registration_tokens")]
        public List<string> RegistrationTokens { get; private set; }

        [JsonProperty(PropertyName = "to")]
        public string Topic { get; private set; }
    }
}
