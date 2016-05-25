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

            var users = Repository.Query<GCMToken>().GetCollection(@event.Token);
            foreach (var user in users)
            {
                var debil = Repository.Query<GCMToken>().GetCollectionItem(@event.Token, user.User);
                if (ReferenceEquals(debil, null) == false)
                    Repository.Query<GCMToken>().Delete(debil);

                Repository.Query<GCMToken>().Delete(user);
            }

            Repository.Query<GCMToken>().Save(new GCMToken(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromGCM @event)
        {
            var subscription = Repository.Query<GCMSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (ReferenceEquals(subscription, null) == false)
                Repository.Query<GCMSubscriptions>().Delete(subscription);

            var debilProjection = Repository.Query<GCMToken>().GetCollectionItem(@event.UserId, @event.Token);
            if (ReferenceEquals(debilProjection, null) == false)
                Repository.Query<GCMToken>().Delete(debilProjection);
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

    [DataContract(Name = "4a443e4e-9c0c-4b07-8be8-28545de7d785")]
    public class GCMToken : ICollectionDataTransferObjectItem<string, string>
    {
        public GCMToken() { }

        public GCMToken(string userId, string token)
        {
            (this as ICollectionDataTransferObjectItem<string, string>).Id = userId;
            (this as ICollectionDataTransferObjectItem<string, string>).CollectionId = token;
        }

        public string User { get { return (this as ICollectionDataTransferObjectItem<string, string>).Id; } }

        public string Token { get { return (this as ICollectionDataTransferObjectItem<string, string>).CollectionId; } }

        [DataMember(Order = 1)]
        string ICollectionDataTransferObjectItem<string, string>.Id { get; set; }

        [DataMember(Order = 2)]
        string ICollectionDataTransferObject<string>.CollectionId { get; set; }
    }
}