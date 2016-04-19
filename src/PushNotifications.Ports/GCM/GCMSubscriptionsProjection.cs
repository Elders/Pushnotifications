using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Ports.GCM
{
    public class GCMSubscriptionsProjection : IProjection, IHaveProjectionsRepository,
        IEventHandler<UserSubscribedForGCM>,
        IEventHandler<UserUnSubscribedFromGCM>
    {
        public void Handle(UserSubscribedForGCM @event)
        {
            Repository.Query<GCMSubscriptions>().Save(new GCMSubscriptions(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromGCM @event)
        {
            var subscription = Repository.Query<GCMSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (!ReferenceEquals(subscription, null))
                Repository.Query<GCMSubscriptions>().Delete(subscription);
        }

        public IRepository Repository { get; set; }
    }

    [DataContract(Name = "e0cb75f0-a333-40fb-813e-8702463a484f")]
    public class GCMSubscriptions : ICollectionDataTransferObjectItem<string, string>
    {
        public GCMSubscriptions() { }

        public GCMSubscriptions(string userId, string token)
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