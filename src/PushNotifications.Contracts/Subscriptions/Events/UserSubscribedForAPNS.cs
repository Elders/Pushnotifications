using Elders.Cronus.DomainModeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "c93ea1cb-0960-4190-abcc-9d5ad1b8dca7")]
    public class UserSubscribedForAPNS : IEvent
    {
        UserSubscribedForAPNS() { }

        public UserSubscribedForAPNS(APNSSubscriptionId id, string userId, string token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public APNSSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Token { get; private set; }
    }
}