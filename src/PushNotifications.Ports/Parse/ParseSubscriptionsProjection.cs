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

            var users = Repository.Query<ParseToken>().GetCollection(@event.Token);
            foreach (var user in users)
            {
                var debil = Repository.Query<ParseToken>().GetCollectionItem(@event.Token, user.User);
                if (ReferenceEquals(debil, null) == false)
                    Repository.Query<ParseToken>().Delete(debil);

                Repository.Query<ParseToken>().Delete(user);
            }

            Repository.Query<ParseToken>().Save(new ParseToken(@event.UserId, @event.Token));
        }

        public void Handle(UserUnSubscribedFromParse @event)
        {
            var subscription = Repository.Query<ParseSubscriptions>().GetCollectionItem(@event.Token, @event.UserId);
            if (!ReferenceEquals(subscription, null))
                Repository.Query<ParseSubscriptions>().Delete(subscription);

            var debilProjection = Repository.Query<ParseToken>().GetCollectionItem(@event.UserId, @event.Token);
            if (ReferenceEquals(debilProjection, null) == false)
                Repository.Query<ParseToken>().Delete(debilProjection);
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

    [DataContract(Name = "a1ecfdd3-ef6f-464c-be8b-efe4b71af823")]
    public class ParseToken : ICollectionDataTransferObjectItem<string, string>
    {
        public ParseToken() { }

        public ParseToken(string userId, string token)
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