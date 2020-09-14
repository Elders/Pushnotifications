using System.ComponentModel.DataAnnotations;
using Elders.Cronus;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class PushySubscribeModel
    {
        /// <summary>
        /// URN of the subscriber. This must be string tenant urn
        /// </summary>
        [IsUrn]
        public string Subscriber { get; set; }

        /// <summary>
        /// Registration token
        /// </summary>
        [Required]
        public string Token { get; set; }

        public Subscribe AsSubscribeCommand()
        {
            var urn = AggregateUrn.Parse(Subscriber, Urn.Uber);

            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.Pushy);
            var subscriptionId = SubscriptionId.New(urn.Tenant, subscriptionToken);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new Subscribe(subscriptionId, subscriberId, subscriptionToken);
        }

        public UnSubscribe AsUnSubscribeCommand()
        {
            var urn = AggregateUrn.Parse(Subscriber, Urn.Uber);

            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.Pushy);
            var subscriptionId = SubscriptionId.New(urn.Tenant, subscriptionToken);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new UnSubscribe(subscriptionId, subscriberId, subscriptionToken);
        }
    }
}
