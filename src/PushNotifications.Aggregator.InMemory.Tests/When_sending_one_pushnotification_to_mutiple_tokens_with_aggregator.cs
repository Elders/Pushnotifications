using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications;
using System.Threading;
using System.Collections.Generic;
using PushNotifications.Subscriptions;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    [Subject(typeof(InMemoryPushNotificationAggregator))]
    public class When_sending_one_pushnotification_to_mutiple_tokens_with_aggregator
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromSeconds(1);
            countOfRecipientsBeforeFlush = int.MaxValue;
            concreateDelivery = new TestDeliveryWithInMemoryAggregator(timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = DateTimeOffset.MaxValue;
            var notificationData = new Dictionary<string, object>();
            notificationData.Add("test", "test");

            t1 = new SubscriptionToken("t1", SubscriptionType.FireBase);
            t2 = new SubscriptionToken("t2", SubscriptionType.FireBase);
            n1 = new NotificationForDelivery(PushNotificationId.New("elders", "n1"), new NotificationPayload("title-1", "body"), notificationData, expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            concreateDelivery.SendAsync(new List<SubscriptionToken>() { t1 }, n1);
            concreateDelivery.SendAsync(new List<SubscriptionToken>() { t2 }, n1);
            Thread.Sleep((int)timeSpanBeforeFlush.TotalMilliseconds * 3);
        };

        It should_send_correct_number_of_notifications = () => concreateDelivery.Store.Count().ShouldEqual(2);

        It should_send_to_correct_first_token = () => concreateDelivery.Store.Where(x => x.Key.Equals(t1)).Count().ShouldEqual(1);

        It should_send_to_correct_second_token = () => concreateDelivery.Store.Where(x => x.Key.Equals(t2)).Count().ShouldEqual(1);

        It should_send_to_correct_notifications = () => { concreateDelivery.Store.First().Value.ShouldEqual(n1); };

        static TestDeliveryWithInMemoryAggregator concreateDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static DateTimeOffset expirationDateOfNotification;

        static SubscriptionToken t1;
        static SubscriptionToken t2;
        static NotificationForDelivery n1;
    }
}
