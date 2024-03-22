using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions.Events;

[DataContract(Name = "a67bdb79-4f06-4856-8f04-05f38ce2e6cf")]
public sealed class UnsubscribedFromTopic : IEvent
{
    UnsubscribedFromTopic() { }

    public UnsubscribedFromTopic(TopicSubscriptionId topicSubscriptionId, DateTimeOffset timestamp)
    {
        if (topicSubscriptionId.IsValid() == false) throw new ArgumentException(nameof(topicSubscriptionId));

        Id = topicSubscriptionId;
        Timestamp = timestamp;
    }

    [DataMember(Order = 1)]
    public TopicSubscriptionId Id { get; private set; }

    [DataMember(Order = 2)]
    public DateTimeOffset Timestamp { get; private set; }
}
