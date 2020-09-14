using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications;
using System.Threading;
using System.Collections.Generic;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    [Subject(typeof(InMemoryPushNotificationAggregator))]
    public class When_sending_pushnotification_with_aggregator_using_timespan_to_flush
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromSeconds(1);
            countOfRecipientsBeforeFlush = int.MaxValue;
            concreateDelivery = new TestDeliveryWithInMemoryAggregator(timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = DateTimeOffset.MaxValue;
            countOfRecipients = 10;
            var notificationData = new Dictionary<string, object>();
            notificationData.Add("test", "test");
            notification = new NotificationForDelivery(PushNotificationId.New("elders"), new NotificationPayload("title", "body"), notificationData, expirationDateOfNotification, true);
        };

        Because of = () =>
        {
            Helper.Send(concreateDelivery, countOfRecipients, notification);
            Thread.Sleep((int)timeSpanBeforeFlush.TotalMilliseconds * 3);
        };

        It should_have_sent_notifications_to_all_recipients_after_waiting_for_the_timespan = () => concreateDelivery.Store.Count().ShouldEqual(countOfRecipients);

        static TestDeliveryWithInMemoryAggregator concreateDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static DateTimeOffset expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationForDelivery notification;
    }
}
