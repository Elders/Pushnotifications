using PushNotifications.Delivery;
using PushNotifications.Subscriptions;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface ITopicSubscriptionManager
    {
        public SubscriptionType Platform { get; }

        Task<SubscribeUnsubscribeHandler> SubscribeToTopicAsync(SubscriptionToken token, Topic topic);

        Task<SubscribeUnsubscribeHandler> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic);
    }
}
