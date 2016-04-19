using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Ports.Parse
{
    public class ParseSubscriptionsProjection : IProjection, IHaveProjectionsRepository,
        IEventHandler<UserSubscribedForParse>,
        IEventHandler<UserUnSubscribedFromParse>
    {
        public void Handle(UserSubscribedForParse @event)
        {
            Repository.Query<ParseSubscriptions>().Save(new ParseSubscriptions(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromParse @event)
        {
            var subscription = Repository.Query<ParseSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (!ReferenceEquals(subscription, null))
                Repository.Query<ParseSubscriptions>().Delete(subscription);
        }

        public IRepository Repository { get; set; }
    }

    [DataContract(Name = "6804a24b-9fed-4179-b0c0-41501f33d18e")]
    public class ParseSubscriptions : ICollectionDataTransferObjectItem<string, string>
    {
        public ParseSubscriptions() { }

        public ParseSubscriptions(string userId, string token)
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