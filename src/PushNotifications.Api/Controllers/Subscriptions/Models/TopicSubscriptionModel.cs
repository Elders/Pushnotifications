using Elders.Cronus;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;
using System.ComponentModel.DataAnnotations;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class TopicSubscriptionModel
    {
        /// <summary>
        /// URN of the subscriber. This must be string tenant urn
        /// </summary>
        [Required, IsUrn]
        public string SubscriberId { get; set; }

        [Required]
        public string Topic { get; set; }

        public string Application { get; set; } = "vapt"; // This is temp solution, because the PushNotifications .NET client don't support APPLICATION as input parameter.

        public SubscribeToTopic AsSubscribeToTopicCommand()
        {
            var urn = AggregateRootId.Parse(SubscriberId.UberDecode());
            var id = new DeviceSubscriberId(urn.Tenant, urn.Id, Application);

            var topic = new Topic(Topic);

            var topicSubscriptionId = new TopicSubscriptionId(urn.Tenant, topic, id);
            return new SubscribeToTopic(topicSubscriptionId, id, topic);
        }

        public UnsubscribeFromTopic AsUnSubscribeFromTopicCommand()
        {
            var urn = AggregateRootId.Parse(SubscriberId.UberDecode());
            var id = new DeviceSubscriberId(urn.Tenant, urn.Id, Application);

            var topic = new Topic(Topic);
            var topicSubscriptionId = new TopicSubscriptionId(urn.Tenant, topic, id);
            return new UnsubscribeFromTopic(topicSubscriptionId, id, topic);
        }
    }
}
