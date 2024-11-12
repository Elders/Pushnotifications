using PushNotifications.Delivery;
using PushNotifications.Subscriptions;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface ITopicSubscriptionManager
    {
        public SubscriptionType Platform { get; }

        Task<SubscribeUnsubscribeResultModel> SubscribeToTopicAsync(SubscriptionToken token, Topic topic);

        Task<SubscribeUnsubscribeResultModel> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic);
    }
}
