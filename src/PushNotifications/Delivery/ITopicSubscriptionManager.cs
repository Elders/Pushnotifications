using PushNotifications.Subscriptions;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface ITopicSubscriptionManager
    {
        public SubscriptionType Platform { get; }

        Task<bool> SubscribeToTopicAsync(SubscriptionToken token, Topic topic);

        Task<bool> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic);

        Task<object> TryUnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic);

        Task<object> TrySubscribeToTopicAsync(SubscriptionToken token, Topic topic);
    }
}
