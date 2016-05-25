using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.APNS
{
    public class APNSSubscriptionsProjection : IProjection, IHaveProjectionsRepository,
        IEventHandler<UserSubscribedForAPNS>,
        IEventHandler<UserUnSubscribedFromAPNS>
    {
        public void Handle(UserSubscribedForAPNS @event)
        {
            Repository.Query<APNSSubscriptions>().Save(new APNSSubscriptions(@event.UserId, @event.Token, 0));
            var users = Repository.Query<APNSToken>().GetCollection(@event.Token);
            foreach (var user in users)
            {
                var debil = Repository.Query<APNSSubscriptions>().GetCollectionItem(@event.Token, user.User);
                if (ReferenceEquals(debil, null) == false)
                    Repository.Query<APNSSubscriptions>().Delete(debil);

                Repository.Query<APNSToken>().Delete(user);
            }

            Repository.Query<APNSToken>().Save(new APNSToken(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromAPNS @event)
        {
            var subscription = Repository.Query<APNSSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (ReferenceEquals(subscription, null) == false)
                Repository.Query<APNSSubscriptions>().Delete(subscription);

            var debilProjection = Repository.Query<APNSToken>().GetCollectionItem(@event.UserId, @event.Token);
            if (ReferenceEquals(debilProjection, null) == false)
                Repository.Query<APNSToken>().Delete(debilProjection);
        }

        public IRepository Repository { get; set; }
    }

    [DataContract(Name = "c3dab7f5-01fe-4cdf-929a-a827336814aa")]
    public class APNSSubscriptions : ICollectionDataTransferObjectItem<string, string>
    {
        public APNSSubscriptions() { }

        public APNSSubscriptions(string userId, string token, int badge)
        {
            (this as ICollectionDataTransferObjectItem<string, string>).Id = token;
            (this as ICollectionDataTransferObjectItem<string, string>).CollectionId = userId;
            Badge = badge;
        }

        public string Token { get { return (this as ICollectionDataTransferObjectItem<string, string>).Id; } }

        public string User { get { return (this as ICollectionDataTransferObjectItem<string, string>).CollectionId; } }

        [DataMember(Order = 1)]
        string ICollectionDataTransferObjectItem<string, string>.Id { get; set; }

        [DataMember(Order = 2)]
        string ICollectionDataTransferObject<string>.CollectionId { get; set; }

        [DataMember(Order = 3)]
        public int Badge { get; set; }


        public static IEqualityComparer<APNSSubscriptions> Comparer { get { return new APNSSubscriptionsComparer(); } }

        public class APNSSubscriptionsComparer : IEqualityComparer<APNSSubscriptions>
        {
            public bool Equals(APNSSubscriptions x, APNSSubscriptions y)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
                return x.User == y.User && x.Token == y.Token;
            }

            public int GetHashCode(APNSSubscriptions obj)
            {
                return (117 ^ obj.User.GetHashCode()) ^ obj.Token.GetHashCode();
            }
        }
    }

    [DataContract(Name = "639bb6f0-ce27-4319-a51c-75bbdc5b7815")]
    public class APNSToken : ICollectionDataTransferObjectItem<string, string>
    {
        public APNSToken() { }

        public APNSToken(string userId, string token)
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