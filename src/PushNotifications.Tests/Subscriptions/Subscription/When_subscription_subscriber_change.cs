﻿using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscription_subscriber_change
    {
        Establish context = () =>
        {
            id = new DeviceSubscriptionId("elders", "id");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            newSubscriberId = new DeviceSubscriberId("elders", "kv2", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.Subscribe(newSubscriberId, DateTimeOffset.UtcNow);

        It should_have_correct_new_subscriber = () => ar.ShouldHaveEvent<Subscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(newSubscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static DeviceSubscriberId newSubscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
