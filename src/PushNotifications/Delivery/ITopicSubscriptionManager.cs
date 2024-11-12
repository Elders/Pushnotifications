using PushNotifications.Delivery;
using PushNotifications.Subscriptions;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface ITopicSubscriptionManager
    {
        public SubscriptionType Platform { get; }

        Task<SubscribeUnsubscribeResult> SubscribeToTopicAsync(SubscriptionToken token, Topic topic);

        Task<SubscribeUnsubscribeResult> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic);
    }
}
