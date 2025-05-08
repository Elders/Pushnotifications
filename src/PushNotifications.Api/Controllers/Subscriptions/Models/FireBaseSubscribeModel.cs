using System.ComponentModel.DataAnnotations;
using PushNotifications.Subscriptions.Commands;
using PushNotifications.Subscriptions;
using Elders.Cronus;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class FireBaseSubscribeModel
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

        public Subscribe AsSubscribeCommand(ApiContext context)
        {
            var urn = context.CurrentUser.UserId ?? AggregateRootId.Parse(Subscriber.UberDecode());

            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.FireBase);
            var subscriptionId = new DeviceSubscriptionId(urn.Tenant, subscriptionToken);
            var subscriberId = new DeviceSubscriberId(urn.Tenant, urn.Id, context.Application);
            return new Subscribe(subscriptionId, subscriberId, subscriptionToken);
        }

        public UnSubscribe AsUnSubscribeCommand(ApiContext context)
        {
            var urn = context.CurrentUser.UserId ?? AggregateRootId.Parse(Subscriber.UberDecode());

            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.FireBase);
            var subscriptionId = new DeviceSubscriptionId(urn.Tenant, subscriptionToken);
            var subscriberId = new DeviceSubscriberId(urn.Tenant, urn.Id, context.Application);
            return new UnSubscribe(subscriptionId, subscriberId, subscriptionToken);
        }
    }
}
