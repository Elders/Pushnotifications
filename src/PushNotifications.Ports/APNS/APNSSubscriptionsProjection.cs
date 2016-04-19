using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.APNS
{
    public class APNSSubscriptionsProjection : IProjection, IHaveProjectionsRepository,
        IEventHandler<UserSubscribedForAPNS>,
        IEventHandler<UserUnSubscribedFromAPNS>
    {
        public void Handle(UserSubscribedForAPNS @event)
        {
            Repository.Query<APNSSubscriptions>().Save(new APNSSubscriptions(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromAPNS @event)
        {
            var subscription = Repository.Query<APNSSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (!ReferenceEquals(subscription, null))
                Repository.Query<APNSSubscriptions>().Delete(subscription);
        }

        public IRepository Repository { get; set; }
    }

    [DataContract(Name = "c3dab7f5-01fe-4cdf-929a-a827336814aa")]
    public class APNSSubscriptions : ICollectionDataTransferObjectItem<string, string>
    {
        public APNSSubscriptions() { }

        public APNSSubscriptions(string userId, string token)
        {
            (this as ICollectionDataTransferObjectItem<string, string>).Id = token;
            (this as ICollectionDataTransferObjectItem<string, string>).CollectionId = userId;
        }

        public string Token { get { return (this as ICollectionDataTransferObjectItem<string, string>).Id; } }

        public string User { get { return (this as ICollectionDataTransferObjectItem<string, string>).CollectionId; } }

        [DataMember(Order = 1)]
        string ICollectionDataTransferObjectItem<string, string>.Id { get; set; }

        [DataMember(Order = 2)]
        string ICollectionDataTransferObject<string>.CollectionId { get; set; }
    }
}