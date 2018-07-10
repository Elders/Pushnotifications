using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "7978b129-3552-457b-9e58-b6a961d1d56a")]
    public class Topic : ValueObject<Topic>
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
