using Machine.Specifications;
using System;
using System.Linq;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using System.Collections.Generic;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    [Subject(typeof(InMemoryPushNotificationAggregator))]
    public class When_sending_pushnotification_with_aggregator_using_tokens_count_to_flush_and_flush_count_is_not_reached
    {
        Establish ctx = () =>
        {
            timeSpanBeforeFlush = TimeSpan.FromDays(1);
            countOfRecipientsBeforeFlush = 5;
            concreateDelivery = new TestDeliveryWithInMemoryAggregator(timeSpanBeforeFlush, countOfRecipientsBeforeFlush);

            expirationDateOfNotification = Timestamp.JudgementDay();
            countOfRecipients = 4;
            var notificationData = new Dictionary<string, object>();
            notificationData.Add("test", "test");
            notification = new NotificationForDelivery(new PushNotificationId(Guid.NewGuid().ToString(), "elders"), new NotificationPayload("title", "body"), notificationData, expirationDateOfNotification, true);
        };

        Because of = () => Helper.Send(concreateDelivery, countOfRecipients, notification);

        It should_have_sent_zero_notifications = () => concreateDelivery.Store.Count().ShouldEqual(0);

        static TestDeliveryWithInMemoryAggregator concreateDelivery;
        static TimeSpan timeSpanBeforeFlush;
        static int countOfRecipientsBeforeFlush;
        static Timestamp expirationDateOfNotification;
        static int countOfRecipients;
        static NotificationForDelivery notification;
    }
}
