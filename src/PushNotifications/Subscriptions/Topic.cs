using System;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "7978b129-3552-457b-9e58-b6a961d1d56a")]
    public sealed record class Topic
    {
        private Topic() { }

        public Topic(string topic)
        {
            if (string.IsNullOrEmpty(topic)) throw new ArgumentNullException(nameof(topic));

            Value = topic;
        }

        [DataMember(Order = 1)]
        private string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Topic current)
        {
            return current.Value;
        }
    }
}
